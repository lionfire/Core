using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{
    public interface IPersistenceContextProvider
    {
        PersistenceContext GetPersistenceContext(IReference reference);
    }

    public class FilesystemContextProvider: IPersistenceContextProvider
    {
        public PersistenceContext GetPersistenceContext(IReference reference)
        {
            new PersistenceContext
        }
    }
}
