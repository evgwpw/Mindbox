namespace Inec.StateMachine.Storage
{
    /// <summary>
    /// базовый класс для сохранения статусов, в основном реестре или отдельном
    /// </summary>
    public abstract class StorageBase
    {
        /// <summary>
        /// справочный код
        /// </summary>
        public int Code { get; private set; }
        /// <summary>
        /// справочное значение
        /// </summary>
        public string Value { get; private set; }
        /// <summary>
        /// регистр, для которого сохраняем
        /// </summary>
        public int RegisterId { get; private set; }
        /// <summary>
        /// идентификатор объекта
        /// </summary>
        public int ObjectId { get; private set; }
        public int TdInstanceId { get; set; }
        public int StateMachineId { get; private set; }
        protected StorageBase(int stateMachineId, int code, string value, int objectId)
        {
            Code = code;
            Value = value;
            StateMachineId = stateMachineId;
            ObjectId = objectId;
            TdInstanceId = -1;
            RegisterId = (int)StateMachineHelper.GetRegisterIdFromStateMachine(StateMachineId);
        }
        protected StorageBase() { }
        public abstract int Save();
        public static StorageBase Create(int stateMachineId, int code, string value, int objectId)
        {
            var tp = StateMachineHelper.GetStorageType(stateMachineId);
            switch (tp)
            {
                case StorageType.INREGISTRE:
                    return new InRegistryStorage(stateMachineId, code, value, objectId);
                case StorageType.INSEPARATEREGISTRE:
                    return new InSeparateRegistryStorage(stateMachineId, code, value, objectId);
                default:
                    throw new StateMachineException("Не верный тип хранения статусов в машине состояний с id=" + stateMachineId);
            }
        }
        
    }
}
