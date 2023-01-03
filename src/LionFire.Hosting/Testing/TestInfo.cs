using LionFire.ExtensionMethods;
using System.Linq;
using System.Reflection;

namespace LionFire.Hosting;

public static class TestInfo
{
    public static bool? IsTest
    {
        get
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null) return null;

            if (assembly.Modules.Any() && assembly.Modules.First().Name == "testhost.dll") return true;

            return false;
        }
    }
}
