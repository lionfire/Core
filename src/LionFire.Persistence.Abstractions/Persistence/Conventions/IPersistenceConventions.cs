using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{
    public interface IPersistenceConventions
    {
        ItemFlags ResolveItemFlags(string name);
    }
}
