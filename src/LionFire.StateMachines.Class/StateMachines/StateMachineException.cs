using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.StateMachines.Class
{

    public class StateMachineException : Exception
    {
        public StateMachineException() { }
        public StateMachineException(string message) : base(message) { }
        public StateMachineException(string message, Exception inner) : base(message, inner) { }
    }
}
