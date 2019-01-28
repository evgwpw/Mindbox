using System;

namespace Inec.StateMachine.ValidationСycles
{
    /// <summary>
    /// дуга графа
    /// </summary>
    public class GraphArc
    {
        //от машины состояний
        public StateMachineData From { get; private set; }
        //к машине состояний
        public StateMachineData To { get; private set; }

        public GraphArc(StateMachineData from, StateMachineData to)
        {
            if (from == null)
                throw new ArgumentNullException("from");
            if (to == null)
                throw new ArgumentNullException("to");
            From = from;
            To = to;
        }

        public override bool Equals(object obj)
        {
            var t = obj as GraphArc;
            if (t == null)
                return false;
            return From.Equals(t.From) && To.Equals(t.To);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return ((From != null ? From.GetHashCode() : 0) * 397) ^ (To != null ? To.GetHashCode() : 0);
            }
        }
    }
}
