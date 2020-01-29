using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LionFire.Applications.Hosting
{
    // FUTURE: Optimize by enabiling unit test detection from Xunit assembly fixture?
    //  - https://github.com/xunit/samples.xunit/tree/master/AssemblyFixtureExample
    // - https://stackoverflow.com/questions/12379949/how-to-run-setup-code-only-once-in-an-xunit-net-test
    // - https://stackoverflow.com/questions/12976319/xunit-net-global-setup-teardown/16590641
    // - https://stackoverflow.com/questions/3167617/determine-if-code-is-running-as-part-of-a-unit-test/30356080#30356080

    public static class UnitTestingDetection
    {
        /// <summary>
        /// Add/remove unit test assemblies here.  Key: Assembly Full Name, Value: unit test framework name.
        /// </summary>
        public static readonly List<KeyValuePair<string, string>> AssemblyForUnitTestFrameworkList = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("xunit.core","Xunit"),
        };

        public static string CurrentUnitTestFramework
        {
            get
            {
                if (currentUnitTestFramework != null) return currentUnitTestFramework;
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var assembly in assemblies)
                {
                    foreach (var kvp in AssemblyForUnitTestFrameworkList)
                    {
                        if(assembly.FullName.StartsWith(kvp.Key + ","))
                        {
                            currentUnitTestFramework = kvp.Value;
                            return currentUnitTestFramework;
                        }
                    }
                }
                currentUnitTestFramework = "";
                return currentUnitTestFramework;
            }
        }
        private static string currentUnitTestFramework = null; // Empty means scan already complete, didn't find anything.

        public static bool IsInUnitTest => CurrentUnitTestFramework != string.Empty;
    }
}

// NUnit detection:
//using NUnit.Framework;
//if (TestContext.CurrentContext != null)
