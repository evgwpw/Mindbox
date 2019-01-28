using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Inec.StateMachine.ValidationСycles
{
    /// <summary>
    /// представляет список списков для результатов поиска 
    /// циклов или деревьев в графе
    /// </summary>
    public class ListOfListMachines : Collection<Collection<VertexGraph>>
    {
       
    }

    /// <summary>
    /// результат обхода в глубину
    /// </summary>
    public class ResultCircumventionDepth
    {
        /// <summary>
        /// циклы
        /// </summary>
        public ListOfListMachines Cycles { get; private set; }

        public ResultCircumventionDepth()
        {
            Cycles = new ListOfListMachines();
        }

        public void AddCycle(IList<VertexGraph> smd)
        {
            Cycles.Add(new Collection<VertexGraph>(smd));
        }
    }

}