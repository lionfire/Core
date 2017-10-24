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
        public ExecutionStateEx OldState {get;set;}
        public ExecutionStateEx NewState {get;set;}
    }
}
