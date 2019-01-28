using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Inec.StateMachine.Storage;

namespace Inec.StateMachine.ValidationСycles
{
    /// <summary>
    /// список всех машин состояний
    /// </summary>
    public sealed class StateMachineDataList : Collection<StateMachineData>
    {
        public StateMachineDataList()
        {
            Init();
        }

        private void AddWithLock(StateMachineData smd)
        {
            lock (this)
            {
                Add(smd);
            }
        }

        private void Init()
        {
            var data = StateMachineItemList.AllMachines.Where(x => x.Enabled);
            Task.WaitAll((from x in data
                select x.Id
                into tmp
                select (Action) (() => AddWithLock(new StateMachineData(tmp)))
                into fun
                select Task.Factory.StartNew(fun)).ToArray());
        }
    }
}