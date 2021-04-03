using LionFire.Data.LiteDB.Connections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence.LiteFs.Tests
{
    public static class ConnectionOptionsInputData
    {
        public static IEnumerable<object[]> AllForXUnit => All.Select(x => new object[] { x });

        public static string NewDbPath => Path.Combine(Path.GetTempPath(), "UnitTestData-" + Guid.NewGuid() + ".lite");
        public static IEnumerable<LiteDbConnectionOptions> All
        {
            get
            {
                yield return new LiteDbConnectionOptions { Path = NewDbPath };
                yield return new LiteDbConnectionOptions { Path = NewDbPath, Password = "UnitTest" };
                yield return new LiteDbConnectionOptions { Path = NewDbPath, AllowCreateDb = false };
                yield return new LiteDbConnectionOptions { Path = NewDbPath, LockingMode = LiteDb.LockingMode.ProcessSafe };
                yield return new LiteDbConnectionOptions { InMemory = true };
                yield return new LiteDbConnectionOptions { TempDiskDb = true };
                // TODO: Add more?
            }
        }
    }
}
