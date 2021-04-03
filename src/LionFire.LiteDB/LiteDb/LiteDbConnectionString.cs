using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.LiteDb
{
    public static class LiteDbConnectionString
    {
        public static string BuildConnectionString(string path) => path;
        public static string BuildConnectionString(string path = null, bool inMemory = false, bool temp = false, LockingMode lockingMode = LockingMode.ThreadSafe, string password = null, LiteDbSize initialSize = null, bool readOnly = false, bool upgrade = false)
        {
            int locationCount = 0;
            if (path != null) locationCount++;
            if (inMemory) locationCount++;
            if (temp) locationCount++;

            if (path == null && !inMemory && !temp)
            {
                throw new ArgumentException($"{(locationCount == 0 ? "One" : "Only one")} of these must be set: {nameof(path)}, {nameof(inMemory)}, or {nameof(temp)}");
            }

            List<string> list = new();

            if (path != null) { list.Add("Filename=" + path); }
            if (inMemory) { list.Add("Filename=:memory:"); }
            if (temp) { list.Add("Filename=:temp:"); }

            if (lockingMode == LockingMode.ProcessSafe) list.Add("Connection=shared");

            if (password != null) { list.Add($"Password={password}"); }

            if (initialSize != null) { list.Add($"InitialSize={initialSize}"); }

            if (readOnly) { list.Add("ReadOnly=true"); }
            if (upgrade) { list.Add("Upgrade=true"); }

            return list.Any() ? list.Aggregate((x, y) => $"{x};{y}") : null;
        }
    }
}
