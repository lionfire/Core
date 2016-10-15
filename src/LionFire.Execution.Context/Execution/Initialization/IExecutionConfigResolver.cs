using LionFire.Execution.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution.Initialization
{
    public interface IExecutionConfigResolver
    {
        Task<bool> Resolve(ExecutionConfig config);
    }
}
