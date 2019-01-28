using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Inec.StateMachine.Storage
{
    public class TreeRefItemList : Collection<TreeRefItem>
    {
        public TreeRefItemList(IEnumerable<TreeRefItem> list = null)
        {
            if (list != null)
            {
                this.CopyEnunerableToList(list);
            }
        }
    }
}
