using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace LionFire.Data.Connections
{
    /// <summary>
    /// Like ConnectionBase but has a dependency on IOptionsMonitor to get the options from the connection name.
    /// </summary>
    /// <typeparam name="TConnectionOptions"></typeparam>
    public abstract class OptionsConnectionBase<TConnectionOptions, TConcrete> : ConnectionBase<TConnectionOptions, TConcrete>
        //, IDisposable
        where TConnectionOptions : ConnectionOptions<TConnectionOptions>
        where TConcrete : OptionsConnectionBase<TConnectionOptions, TConcrete>
    {
        //IDisposable changeListener;
        public OptionsConnectionBase(string connectionName, IOptionsMonitor<NamedConnectionOptions<TConnectionOptions>> options, ILogger<TConcrete> logger) : base(options.CurrentValue.GetOptionsOrDefault(connectionName), logger)
        {
           
            //changeListener = options.OnChange((newOptions, nameOfNamedConnectionOptions) =>
            //{
            //    // Unfortunately, nameOfNamedConnectionOptions doesn't correspond to connectionName.
            //    //if (nameOfNamedConnectionOptions == connectionName)
            //    //{
            //    //    base.Options = newOptions.Connections[connectionName];
            //    //}
            //});
        }

        //public void Dispose() => changeListener.Dispose();

    }
}
