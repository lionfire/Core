using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Hosting
{
    public class ServiceInfos : List<ServiceInfo>
    {
        
    }

    public class PortServiceInfo : ServiceInfo
    {
        public string PortName { get; set; }

        public int Port { get; set; }

        public PortServiceInfo(IHostEnvironment hostEnvironment, IConfiguration configuration) : base(hostEnvironment, configuration)
        {
        }
    }
}
