using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution.Configuration
{
    public enum ExecutionLocationType
    {
        InProcess,
        CurrentUser,
        // OtherUser,
        LocalMachine,
        OtherMachine,
        Hive,
        Global,
    }

}
