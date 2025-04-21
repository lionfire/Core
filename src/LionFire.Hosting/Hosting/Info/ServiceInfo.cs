using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace LionFire.Hosting;

public class ServiceInfo
{
    public ServiceInfo(IHostEnvironment hostEnvironment, IConfiguration configuration)
    {
        App = hostEnvironment.ApplicationName;
        ReleaseChannel = configuration["ReleaseChannel"];

    }

    public string App { get; set; }

    public int Pid { get; set; } = Process.GetCurrentProcess().Id;

    public string ReleaseChannel { get; set; }

}
