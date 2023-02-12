using LionFire.Applications;
using Microsoft.Extensions.Options;

namespace LionFire.Hosting;

public class SiloServiceNameProvider
{
    public SiloServiceNameProvider(AppInfo appInfo, OrleansClusterConfigProvider orleansClusterConfig)
    {
        if (!appInfo.IsValid) { throw new InvalidOperationException($"AppInfo is invalid: " + appInfo.InvalidReason); }
        ServiceName = @$"silo.{(
            (orleansClusterConfig.ServiceId).ToLower()
                .Replace("lionfire.", "")
                .Replace(oldValue: "silo.", "")
                .Replace(oldValue: ".silo", "")
            )}.services.{appInfo.OrgDomain}";
    }

    public string ServiceName { get; }
}