using System;

namespace LionFire.Rethink
{
    public class RethinkOptions
    {
        public string Host { get; set; }
        public int Port { get; set; } = 28015;
        public int TimeoutSeconds { get; set; } = 60;
    }
}
