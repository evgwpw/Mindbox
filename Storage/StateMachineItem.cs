using System;

namespace Inec.StateMachine.Storage
{
    public class StateMachineItem
    {
        
        /// <summary>
        /// идентификатор
        /// </summary>
        public long Id { get; private set; }
        /// <summary>
        /// идентификатор справочника
        /// </summary>
        public long ReferenceId { get; private set; }
        /// <summary>
        /// наименование
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// статус по умолчанию
        /// </summary>
        public long DefaultStatusId { get; private set; }
        /// <summary>
        /// идентификатор реестра
        /// </summary>
        public long RegisterId { get; private set; }
        /// <summary>
        /// Тип хранения - тип long
        /// </summary>
        public long StorageTypeL { get; private set; }
        /// <summary>
        /// Тип хранения
        /// </summary>
        public StorageType StorageType
        {
            get
            {
                return (StorageType)StorageTypeL;
            }
        }
        /// <summary>
        /// идентификатор атрибута для хранения в том же самом реестре
        /// </summary>
        public long? AttributeId { get; private set; }
        /// <summary>
        /// тип обработки - тип long
        /// </summary>
        public long? ProcessingTypeL { get; private set; }
        /// <summary>
        /// тип обработки
        /// </summary>
        public ProcessingType ProcessingType
        {
            get
            {
                if (!ProcessingTypeL.HasValue)
                    return ProcessingType.None;
                return (ProcessingType)ProcessingTypeL.Value;
            }
        }
        /// <summary>
        /// включена-выключена
        /// </summary>
        public bool Enabled { get; private set; }
        public StateMachineItem(long id, long referenceId, string name, long defaultStatusId, long registerId, long storageType, long? attributeId, long? processingType,
            bool enabled)
        {
            Id = id;
            ReferenceId = referenceId;
            Name = name;
            DefaultStatusId = defaultStatusId;
            RegisterId = registerId;
            StorageTypeL = storageType;
            AttributeId = attributeId;
            ProcessingTypeL = processingType;
            Enabled = enabled;
        }
        public StateMachineItem(ObjectModel.OMStateMachine sm)
        {
            if (sm == null)
                throw new ArgumentNullException("sm");
            Id = sm.Id;
            RegisterId = sm.RegisterId;
            Name = sm.Name;
            DefaultStatusId = sm.DefaultStatusId;
            RegisterId = sm.RegisterId;
            StorageTypeL = sm.StorageType;
            AttributeId = sm.AttributeId;
            ProcessingTypeL = sm.ProcessingType;
            Enabled = sm.Enabled;
        }
        const string Template = @"Id-{0}; ReferenceId-{1}; Name-{2}; DefaultStatusId-{3}; RegisterId-{4}; StorageType-{5}; AttributeId-{6}; ProcessingType-{7}; Enabled-{8};";
        public override string ToString()
        {
            return string.Format(Template, Id, ReferenceId, Name, DefaultStatusId, RegisterId, StorageType, AttributeId, ProcessingType, Enabled);
        }
    }
}
