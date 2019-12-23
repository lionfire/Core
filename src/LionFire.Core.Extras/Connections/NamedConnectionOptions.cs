using System.Collections.Generic;

namespace LionFire.Data
{
    public class NamedConnectionOptions<TConnectionOptions> 
        //: Dictionary<string, TConnectionOptions>
        where TConnectionOptions : ConnectionOptions<TConnectionOptions>
    {

        public Dictionary<string, TConnectionOptions> Connections { get; set; }
    }

    // OLD - works but is a massively deep appsettings.json
    //public class ConnectionManagerOptions<TConnectionOptions> 
    //    where TConnectionOptions : ConnectionOptions<TConnectionOptions>
    //{

    //    public Dictionary<string, TConnectionOptions> Connections { get; set; }
    //}
}
