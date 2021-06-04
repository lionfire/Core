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
    public class LiteDbConnectionOptions : ConnectionOptions<LiteDbConnectionOptions>, IHasConnectionStringRW
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

        public string ConnectionString
        {
            get => LiteDbConnectionString.BuildConnectionString(Path, InMemory, TempDiskDb, LockingMode, Password, InitialSize, ReadOnly, Upgrade);
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    AllowCreateDb = true;
                    DeleteOnClose = false;
                    Password = default;
                    InMemory = default;
                    TempDiskDb = default;
                    LockingMode = LockingMode.ThreadSafe;
                    InitialSize = default;
                    ReadOnly = default;
                    Upgrade = default;
                    return;
                }

                var chunks = value.Split(';');

                foreach (var chunk in chunks)
                {
                    var kvp = chunk.Split('=', 2);
                    if (kvp.Length != 2) { throw new ArgumentException("Each semicolon delimited section must have an equal delimited key value pair"); }

                    switch (kvp[0])
                    {
                        case "Filename":
                            if (kvp[1] == ":memory:")
                            {
                                InMemory = true;
                            }
                            else if (kvp[1] == ":temp:")
                            {
                                TempDiskDb = true;
                            }
                            else
                            {
                                Path = kvp[1];
                            }
                            break;
                        case "Connection":
                            if (kvp[1] == "shared")
                            {
                                LockingMode = LockingMode.ProcessSafe;
                            }
                            else if (kvp[1] == "direct")
                            {
                                LockingMode = LockingMode.ThreadSafe;
                            }
                            break;
                        case "Password":
                            Password = kvp[1];
                            break;
                        case "InitialSize":
                            throw new NotImplementedException("InitialSize parse");
                            //break;
                        case "ReadOnly":
                            ReadOnly = Boolean.Parse(kvp[1]);
                            break;
                        case "Upgrade":
                            Upgrade = Boolean.Parse(kvp[1]);
                            break;
                        default:
                            throw new ArgumentException("unknown key:" + kvp[0]);
                    }
                }
            }
        }
    }
}