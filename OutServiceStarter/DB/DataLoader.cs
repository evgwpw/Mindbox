using System.Linq;
using ObjectModel;
using System.Data;
using Inec.StateMachine.Storage;
using Core.Register.QuerySubsystem;

namespace Inec.StateMachine.OutServiceStarter.DB
{
    /// <summary>
    /// загружает данные из БД для запуска внешних процессов
    /// </summary>
    public class DataLoader : IDataLoader
    {
        public long StateMachineId { get; private set; }
        public long StatusId { get; private set; }
        public long ObjectId { get; private set; }
        public DataLoader(long stateMachineId, long statusId, long objectId)
        {
            StateMachineId = stateMachineId;
            StatusId = statusId;
            ObjectId = objectId;
        }
        /// <summary>
        /// загружает данные для формирования DataSet для вызова web сервиса для создания документа spd
        /// </summary>
        /// <returns></returns>
        public DataTable Table()
        {
            var data = LoadProcess();
            var registerId = StateMachineHelper.GetRegisterIdFromStateMachine(StateMachineId);
            var empIdAttribute = Util.QSQueryUtil.GetRegisterIdAttribute((int)registerId);
            var query = Util.QSQueryUtil.Deserialize(data.QSQuery);
            var cond = query.Condition;
            if (cond == null)//нет условия
            {
                cond = new QSConditionSimple(new QSColumnSimple(empIdAttribute), QSConditionType.Equal, ObjectId);
            }
            else//есть условие
            {
                cond = cond.And(new QSConditionSimple(new QSColumnSimple(empIdAttribute), QSConditionType.Equal, ObjectId));
            }
            query.Condition = cond;
            var res = query.ExecuteQuery();
            return res;
        }
        OMStateMachineOuterProcess LoadProcess()
        {
            var data = OMStateMachineOuterProcess.Where(x => x.StateMachineId == StateMachineId && x.StatusId == StatusId)
                           .SelectAll()
                           .Execute()
                           .FirstOrDefault();
            return data;
        }
    }
}
