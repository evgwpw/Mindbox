using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Shared.Extensions;

namespace Inec.StateMachine.Loging
{
    [Serializable]
    public class LogEntity
    {
        public Guid UniqueId { get; private set; }
        public string Description { get; private set; }
        public int? RegisterId { get; private set; }
        public int? ObjectId { get; private set; }
        public DateTime DateRun { get; private set; }
        public LogEntityType EntityType { get; private set; }
        public int? StateMachineId { get; private set; }
        public LogEntity(string description, LogEntityType entityType = LogEntityType.NONE, int? registerId = null, int? objectId = null, int? stateMachineId = null)
        {
            UniqueId = StateMachineProcessor.UniqueId;
            Description = description;
            RegisterId = registerId.HasValue ? registerId.Value : StateMachineProcessor.RegisterId;
            ObjectId = objectId.HasValue ? objectId.Value : StateMachineProcessor.ObjectId;
            DateRun = DateTime.Now;
            EntityType = entityType;
            StateMachineId = stateMachineId;
        }
        public string ToXMLString()
        {
            return this.SerializeToXml();
        }
        public void WriteToDB()
        {
            try
            {
                Action act = () =>
                {
                    var obj = ObjectModel.OMStateMachineLog.CreateEmpty();
                    obj.DateRun = DateRun;
                    obj.Description = Description;
                    obj.EntityType = EntityType.ToString();
                    obj.ObjectId = Convert.ToInt64(ObjectId);
                    obj.RegisterId = Convert.ToInt64(RegisterId);
                    obj.UniqueId = UniqueId.ToString("d");
                    obj.StateMachineId = StateMachineId;
                    obj.Save();
                };
                act();
              //  System.Threading.Tasks.Task.Factory.StartNew(act);
            }
            catch (Exception exc)
            {
                var sexc = new StateMachineException("Ошибка при сохранении лога машины состояний", exc);
                throw sexc;
            }
        }
    }
}
