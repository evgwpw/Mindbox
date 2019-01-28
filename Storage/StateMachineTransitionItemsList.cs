using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Inec.StateMachine.Storage
{
    /// <summary>
    /// коллекция переходов
    /// </summary>
    public class StateMachineTransitionItemsList:Collection<StateMachineTransitionItem>
    {
        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="list">список переходов</param>
        public StateMachineTransitionItemsList(IEnumerable<StateMachineTransitionItem> list = null)
        {
            if (list != null)
            {
                this.CopyEnunerableToList(list);
            }
        }
    }
}
