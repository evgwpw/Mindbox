using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Inec.StateMachine.Storage;
using Inec.StateMachine.Util;
using ObjectModel;

namespace Inec.StateMachine.ValidationСycles
{
    /// <summary>
    /// данные о узле графа
    /// </summary>
    public class StateMachineData
    {
        public static readonly StateMachineData Empty = new StateMachineData();
        /// <summary>
        /// машина состояний
        /// </summary>
        public long StateMachineId { get; private set; }
        /// <summary>
        /// регистр
        /// </summary>
        public long RegisterId { get; private set; }
        /// <summary>
        /// смежные регистры
        /// </summary>
        public IList<long> ContiguousReesters { get; private set; }
        /// <summary>
        /// реестр, в котором храняться статусы
        /// </summary>
        public long? StatusesRegisterId { get; private set; }
        /// <summary>
        /// родительская машина
        /// </summary>
        public StateMachineData Parent { get; set; }
        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="stateMachineId">машина состояний</param>
        public StateMachineData(long stateMachineId)
        {
            StateMachineId = stateMachineId;
            Init();
        }
        private StateMachineData()
        {}
        /// <summary>
        /// инициализация
        /// </summary>
        private void Init()
        {
            RegisterId = StateMachineHelper.GetRegisterIdFromStateMachine(StateMachineId);
            InitContiguousReesters();
            InitSatusesRegisterId();
        }
        /// <summary>
        /// инициализация реестра хранения
        /// </summary>
        private void InitSatusesRegisterId()
        {
            var ds = StateMachineHelper.GetStorageType((int)StateMachineId);
            StatusesRegisterId = ds == StorageType.INSEPARATEREGISTRE ? 703 : new long?();
        }
        /// <summary>
        /// инициализация смежных реестров
        /// </summary>
        void InitContiguousReesters()
        {
            var data = StateMachineAdjacentRegisters.GetAdjacentRegisters(StateMachineId);
            if (data == null || !data.Any())
            {
                var tmp = OMStateMachineCriteria.Where(x => x.StateMachineId == StateMachineId)
                    .Select(x => x.QsqueryXml)
                    .Execute()
                    .Select(x => QSQueryUtil.Deserialize(x.QsqueryXml));
                data = QSQueryUtil.GetRegisters(tmp).Select(Convert.ToInt64);
            }
            ContiguousReesters = data.ToList();
        }
        public override bool Equals(object obj)
        {
            var tmp = obj as StateMachineData;
            if (tmp == null)
                return false;
            return tmp.StateMachineId == StateMachineId;
        }
        public override int GetHashCode()
        {
            return (StateMachineId.GetHashCode() + RegisterId.GetHashCode()) ^ int.MaxValue;
        }

        private const string Template = @"StateMachineId - {0}; RegisterId - {1}; StatusesRegisterId - {2}; ContiguousReesters - {3}";
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (ContiguousReesters.Count > 0)
            {
                ContiguousReesters.Aggregate(sb, (a, b) => a.AppendLine(b.ToString(CultureInfo.InvariantCulture)));
            }
            return string.Format(Template, StateMachineId, RegisterId, StatusesRegisterId, sb);
        }
    }
}