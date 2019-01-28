using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Inec.StateMachine.Storage
{
    public class CriteriaItemList : Collection<CriteriaItem>
    {
        /// <summary>
        /// идентификатор машины состояний
        /// </summary>
        public int StateMachineId { get; private set; }
        /// <summary>
       
        
      //  public int StatusId { get; private set; }
        /// <summary>
        /// отсортированный список критериев для этой машины состояний
        /// </summary>
        public IEnumerable<CriteriaItem> OrderedList
        {
            get
            {
                return this.OrderBy(x => x.Order);
            }
        }
        public CriteriaItemList(int stateMachineId/*, int statusId*/, IEnumerable<CriteriaItem> list = null)
        {
            StateMachineId = stateMachineId;
         //   StatusId = statusId;
            if (list != null)
            { 
                foreach (var i in list)
                {
                    CheckOrder(i);
                    Add(i);
                }
            }
        }
        void CheckOrder(CriteriaItem item)
        {
            return;
            if (this.Any(x => x.Order == item.Order))
                throw new StateMachineException("Значание с порядком " + item.Order + " уже содержиться в списке");
        }
       
    }
}
