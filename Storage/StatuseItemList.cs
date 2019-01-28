using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Inec.StateMachine.Storage
{
    public class StatuseItemList : Collection<StatuseItem>
    {
        public static readonly StatuseItemList Empty = new StatuseItemList(-1);
        public int StateMachineId { get; private set; }
        public StatuseItemList(int stateMachineId, IEnumerable<StatuseItem> list = null)
        {
            StateMachineId = stateMachineId;
            if (list != null)
            { 
                this.CopyEnunerableToList(list);
            }
        }
    }
}
