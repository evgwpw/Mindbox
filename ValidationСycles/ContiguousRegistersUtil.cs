using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectModel;
using Inec.StateMachine.Util;

namespace Inec.StateMachine.ValidationСycles
{
    /// <summary>
    /// манипуляции со смежными реестрами
    /// </summary>
    public class ContiguousRegistersUtil
    {
        /// <summary>
        /// проверка смежных реестров для машины состояний
        /// </summary>
        /// <param name="stateMachineId"></param>
        public static void CheckRegisters(long stateMachineId)
        {
            var savedItems = OMStateMachineAdjacentRegs.Where(x => x.StateMachineId == stateMachineId)
                .Select(x => x.RegisterId)
                .Execute()
                .Select(x => x.RegisterId)
                .OrderBy(x => x);
            var tmp = OMStateMachineCriteria.Where(x => x.StateMachineId == stateMachineId)
                    .Select(x => x.QsqueryXml)
                    .Execute()
                    .Select(x => QSQueryUtil.Deserialize(x.QsqueryXml));
            var computedItems = QSQueryUtil.GetRegisters(tmp)
                .Select(Convert.ToInt64)
                .OrderBy(x => x);
            var except1 = computedItems.Except(savedItems);//которые есть в вычисленных и нет в сохраненных - надо сохранить
            var except2 = savedItems.Except(computedItems);//которые есть в сохраненных и нет в вычисленных - надо удалить
            SaveItems(except1, stateMachineId);
            DeleteItems(except2, stateMachineId);
        }

        static void SaveItems(IEnumerable<long> src, long stateMachineId)
        {
            foreach (var l in src)
            {
                var item = OMStateMachineAdjacentRegs.CreateEmpty();
                item.RegisterId = l;
                item.StateMachineId = stateMachineId;
                item.IsUsed = true;
                item.Save();
            }
        }

        static void DeleteItems(IEnumerable<long> src, long stateMachineId)
        {
            foreach (var l in src)
            {
                var l1 = l;
                var tmp = OMStateMachineAdjacentRegs.Where(x => x.RegisterId == l1 && x.StateMachineId == stateMachineId)
                    .SelectAll()
                    .Execute()
                    .FirstOrDefault();
                if (tmp != null)
                {
                    tmp.Destroy();
                }
            }
        }
    }
}
