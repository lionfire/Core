using LionFire.Data.Connections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LionFire.MagicOnion_;

public class MagicOnionConnectionManager : ConnectionManager<MagicOnionConnection, MagicOnionConnectionOptions>
{
    public MagicOnionConnectionManager(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public MagicOnionConnectionManager(IOptionsMonitor<NamedConnectionOptions<MagicOnionConnectionOptions>> options, ILogger logger, IServiceProvider serviceProvider) : base(options, logger, serviceProvider)
    {
    }
}

