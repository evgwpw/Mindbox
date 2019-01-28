using System;
using System.Linq;
using Core.ErrorManagment;
using System.Threading.Tasks;
using Core.Register;

namespace Inec.StateMachine.Storage
{
    /// <summary>
    /// сохраняем в том же самом реестре
    /// </summary>
    public class InRegistryStorage : StorageBase
    {
        /// <summary>
        /// идентификатор атрибута
        /// </summary>
        public int AttributeId { get; private set; }
        /// <summary>
        /// сохранять ли историю, ставим соотвествующую S_
        /// </summary>
        public bool IsSaveHistory { get; private set; }
        public InRegistryStorage(int stateMachineId, int code, string value, int objectId)
            : base(stateMachineId, code, value, objectId)
        {
            AttributeId = StateMachineHelper.GetAttributeStarageFromStateMachineId(StateMachineId);
            IsSaveHistory = StateMachineHelper.IsSaveHistory(StateMachineId);
        }
        public override int Save()
        {
            try
            {
                var regObj = new RegisterObject(RegisterId, ObjectId) {HandleCoreEvents = false};
                regObj.SetAttributeValue(AttributeId, Value, Code);
                var sPo = GetS_PO_();
                regObj.HandleCoreEvents = false;
                int res;
                if (sPo != null)
                {
                    res = RegisterStorage.Save(regObj, TdInstanceId, sPo.Item1, sPo.Item2);
                }
                else
                {
                    res = RegisterStorage.Save(regObj, TdInstanceId);
                }
                var checker = new OutServiceStarter.Checker(Code, StateMachineId, ObjectId);
                Task.Factory.StartNew(checker.CheckAndRun);
                return res;
            }
            catch (Exception exc)
            {
                var sexc = new StateMachineException("Не удалось сохранить значение статуса в атрибуты реестра", exc);
                Task.Factory.StartNew(() => ErrorManager.LogErrorInWinSrvEnv(sexc));
                throw sexc;
            }
        }
        /// <summary>
        /// находим даты S_ и PO_, нужны для апдейта
        /// </summary>
        /// <returns></returns>
        Tuple<DateTime, DateTime> GetS_PO_()
        {
            if (IsSaveHistory)
            {
                return null;
            }
            var dt = DateTime.Now;
            var data = RegisterStorage.GetObjectHistory(ObjectId, RegisterId)
                .Rows.Cast<System.Data.DataRow>()
                .Select(r => new { S_ = Convert.ToDateTime(r["S_"]), PO_ = Convert.ToDateTime(r["PO_"]) })
                .FirstOrDefault(x => x.PO_ > dt && x.S_ <= dt);
            if (data == null)
                throw new StateMachineException("Не удалось найти историю для реестра " + RegisterId + " с id=" + ObjectId);
            return Tuple.Create(data.S_, data.PO_);
                
        }
    }
}
