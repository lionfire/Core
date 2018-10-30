using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;

namespace LionFire.Persistence
{
    public static class PersistenceOperationExtensions
    {
        public static void SetPath(this PersistenceOperation persistenceOperation, string path)
        {
            persistenceOperation.Reference = (PathReference)path;
        }
    }
}
