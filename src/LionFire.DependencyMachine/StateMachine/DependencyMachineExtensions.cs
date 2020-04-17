using LionFire.Structures;
using System;

namespace LionFire.DependencyMachine
{
    internal static class DependencyMachineExtensions
    {
        public static string KeyForContributed(this object contributed)
        {
            if (contributed == null) throw new ArgumentNullException();
            if (contributed is string s) return s;

            return contributed.GetType().Name + ":" + (contributed as IKeyed)?.Key ?? contributed.ToString();
        }

    }
}
