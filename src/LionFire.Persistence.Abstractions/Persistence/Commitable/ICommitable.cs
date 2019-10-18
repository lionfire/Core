using LionFire.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public interface ICommitResult : IPersistenceResult, ISuccessResult { }
    
    public interface ICommitable
    {
        /// <summary>
        /// Submit pending changes to the underlying store
        /// </summary>
        Task<ICommitResult> Commit();

    }
}
