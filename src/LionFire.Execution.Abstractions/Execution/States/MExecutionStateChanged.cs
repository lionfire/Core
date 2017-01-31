using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Messaging;

namespace LionFire.Execution
{
    public class MExecutionStateChanged : IMessage
    {
        public object Source { get; set; }
        public ExecutionState OldState {get;set;}
        public ExecutionState NewState {get;set;}
    }
}
