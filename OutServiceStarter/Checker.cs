using System.Linq;
using ObjectModel;

namespace Inec.StateMachine.OutServiceStarter
{
    /// <summary>
    /// проверяет, нужно ли запускать внешний процесс для этого статуса, если нужно - запускает
    /// </summary>
    public class Checker
    {
        public long StatusId { get; private set; }
        public long StateMachineId { get; private set; }
        public long ObjectId { get; private set; }
        public Checker(long statusId, long stateMachineId, int objectId)
        {
            StateMachineId = stateMachineId;
            StatusId = statusId;
            ObjectId = objectId;
        }
        public void CheckAndRun()
        {
            new Loging.LogEntity(string.Format("Проверка на запуск внешних процессов для StateMachineId - {0}; ObjectId - {1}; StatusId - {2}", StateMachineId, ObjectId, StatusId)).WriteToDB();
            var checkedStatuses = OMStateMachineOuterProcess.Where(x => x.StateMachineId == StateMachineId && x.StatusId == StatusId)
                .SelectAll()
                .Execute()
                .Any();
            if (checkedStatuses)
            {
                new Loging.LogEntity(string.Format("Запуск внешнего процесса для StateMachineId - {0}; ObjectId - {1}; StatusId - {2}", StateMachineId, ObjectId, StatusId)).WriteToDB();
                Runner run = new Runner(StatusId, StateMachineId, ObjectId);
                run.Run();
            }
            else
            {
                new Loging.LogEntity(string.Format("Нет внешнего процесса для StateMachineId - {0}; ObjectId - {1}; StatusId - {2}", StateMachineId, ObjectId, StatusId)).WriteToDB();
            }
        }
    }
}
