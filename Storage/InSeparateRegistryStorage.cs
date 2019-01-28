using System.Linq;

namespace Inec.StateMachine.Storage
{
    /// <summary>
    /// сохраняем в отдельном реестре
    /// </summary>
    public class InSeparateRegistryStorage : StorageBase
    {
        public InSeparateRegistryStorage(int stateMachineId, int code, string value, int objectId) : base(stateMachineId, code, value, objectId) { }
        public override int Save()
        {
            var s = GetObject();
            if (s == null)
                return -1;
            s.Statuscode = Code;
            s.Statusvalue = Value;
            s.Registerid = RegisterId;
            s.Objectid = ObjectId;
            s.StateMachineId = StateMachineId;
            var res = s.Save(TdInstanceId);
            var checker = new OutServiceStarter.Checker(Code, StateMachineId, ObjectId);
            new Loging.LogEntity(string.Format("Создали Checker Code - {0}; StateMachineId - {1}; ObjectId - {2}", Code, StateMachineId, ObjectId)).WriteToDB();
            System.Threading.Tasks.Task.Factory.StartNew(checker.CheckAndRun);
            return res;
        }
        ObjectModel.OMStateMachineStatuses GetObject()
        {
            var res = ObjectModel.OMStateMachineStatuses.Where(x => x.Objectid == ObjectId && x.Registerid == RegisterId)
                .SelectAll().Execute().FirstOrDefault();
            if (res == null)
                return ObjectModel.OMStateMachineStatuses.CreateEmpty();
            if (res.Statuscode == Code)
                return null;
            return res;
        }
    }
}
