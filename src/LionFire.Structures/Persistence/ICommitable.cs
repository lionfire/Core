using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public interface ICommitable
    {
        /// <summary>
        /// Submit pending changes to the underlying store
        /// </summary>
        /// <param name="persistenceContext"></param>
        Task Commit(object persistenceContext = null);
    }
}
