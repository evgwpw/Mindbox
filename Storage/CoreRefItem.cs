using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inec.StateMachine.Storage
{
    public class CoreRefItem
    {
        static Tuple<int, string> _GetRefItem(int itemId)
        {
            var tmp = ObjectModel.Core.Shared.OMReferenceItem.Where(x => x.ItemId == itemId)
                .Select(x => x.Value)
                .Execute()
                .Select(x => x.Value)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(tmp))
                throw new StateMachineException("Не удалось найти статус с идентификаторм = " + itemId);
            return Tuple.Create(itemId, tmp);
        }
        static Func<int, Tuple<int, string>> __GetRefItem = new Func<int, Tuple<int, string>>(_GetRefItem);
        /// <summary>
        /// получаеи Id и Value статуса
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static Tuple<int, string> GetRefItem(int itemId)
        {
            return __GetRefItem(itemId);
        }
    }
}
