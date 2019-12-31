using LionFire.Vos.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Environment
{
    public static class VobEnvironmentExtensions
    {
        public static VobEnvironment Environment(this IVob vob) 
            => vob.GetOrAddOwn<VobEnvironment>();
    }
}
