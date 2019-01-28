using System;

namespace Inec.StateMachine.ValidationСycles
{
    /// <summary>
    /// вершина графа
    /// </summary>
    public class VertexGraph
    {
        public static readonly VertexGraph Empty = new VertexGraph(StateMachineData.Empty);
        public  StateMachineData Vertex { get; private set; }
        public bool IsVisited { get; private set; }

        public VertexGraph(StateMachineData vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException("vertex");
            Vertex = vertex;
        }
        /// <summary>
        /// то, что посещена, можно установить только 1 раз
        /// </summary>
        public void SetVisited()
        {
            IsVisited = true;
        }
        public static implicit operator VertexGraph(StateMachineData smd)
        {
            return Equals(smd, StateMachineData.Empty) ? Empty : new VertexGraph(smd);
        }

        public static implicit operator StateMachineData(VertexGraph vg)
        {
            return Equals(vg, Empty) ? StateMachineData.Empty : vg.Vertex;
        }

        public override bool Equals(object obj)
        {
            var t = obj as VertexGraph;
            if (t == null)
                return false;
            return Vertex.Equals(t.Vertex);
        }

        public override int GetHashCode()
        {
            return Vertex.GetHashCode();
        }
    }
}
