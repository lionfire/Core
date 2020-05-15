using LionFire.Structures;
using System;

namespace LionFire.DependencyMachines
{
    public static class DependencyMachineKeyExtensions
    {
        public static string ToDependencyKey(this object contributed)
        {
            if (contributed == null) throw new ArgumentNullException();
            if (contributed is string s) return s;

            return contributed.GetType().Name + ":" + (contributed as IKeyed)?.Key ?? contributed.ToString();
        }
        public static string? ToNullableDependencyKey(this object contributed)
        {
            if (contributed == null) return null;
            if (contributed is string s) return s;

            return contributed.GetType().Name + ":" + (contributed as IKeyed)?.Key ?? contributed.ToString();
        }
    }
}
