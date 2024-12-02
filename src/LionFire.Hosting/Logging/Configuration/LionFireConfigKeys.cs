namespace LionFire.Configuration.Logging;

public class LionFireConfigKeys
{
    public static class Log
    {
        public const string Section = "LionFire:Logging";
        public const string Dir = "LionFire:Logging:File:Dir";
        public const string TestDir = "LionFire:Logging:File:TestDir";
        public const string Enabled = "LionFire:Logging:File:Enabled";
        public const string LogStart = "LionFire:Logging:LogStart";

        public static string SinkEnabled(string sink) => $"{Section}:{sink}:Enabled";
    }
}

//public class LionFireLogOptions
//{
//    public string Dir { get; set; }
//    public bool Enabled { get; set; }
//    public string TestDir { get; set; }
//}
