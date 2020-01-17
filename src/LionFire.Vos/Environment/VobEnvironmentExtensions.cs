using LionFire.Vos.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Environment
{
    public static class VobEnvironmentExtensions
    {
        public static VobEnvironment Environment(this IVob vob)
            => vob.GetNextOrCreateAtRoot<VobEnvironment>();
        public static object Environment(this IVob vob, string key)
            => vob.GetNextOrCreateAtRoot<VobEnvironment>()[key];
        public static object Environment(this IVob vob, string key, object value)
            => vob.GetNextOrCreateAtRoot<VobEnvironment>()[key] = value;
    }
}
