using LionFire.Data;
using LionFire.Data.Connections;
using LionFire.LiteDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Data.LiteDB.Connections
{
    public class LiteDbConnectionOptions : ConnectionOptions<LiteDbConnectionOptions>, IHasConnectionString
    {
        /// <summary>
        /// Default: true
        /// </summary>
        public bool AllowCreateDb { get; set; } = true;
        public bool DeleteOnClose { get; set; } = false;

        public string Password { get; set; }
        public string Path { get; set; }
        public bool InMemory { get; set; }
        public bool TempDiskDb { get; set; }
        public LockingMode LockingMode { get; set; } = LockingMode.ThreadSafe;
        public LiteDbSize InitialSize { get; set; }
        public bool ReadOnly { get; set; }
        public bool Upgrade { get; set; }

        public override string ConnectionString
            => LiteDbConnectionString.BuildConnectionString(Path, InMemory, TempDiskDb, LockingMode, Password, InitialSize, ReadOnly, Upgrade);
    }
}