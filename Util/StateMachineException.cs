using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inec.StateMachine
{
    /// <summary>
    /// ошибки машины состояний
    /// </summary>
    [Serializable]
    public class StateMachineException : Exception
    {
        public StateMachineException() { }
        public StateMachineException(string message) : base(message) { }
        public StateMachineException(string message, Exception inner) : base(message, inner) { }
        protected StateMachineException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) 
            : base(info, context) { }
    }
}
