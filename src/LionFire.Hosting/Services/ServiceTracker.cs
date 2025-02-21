using DynamicData;

namespace LionFire.Hosting.Services;

public class ServiceStatusAdapter : IProvidesServiceStatus
{
    public ServiceStatusAdapter(object obj)
    {
        ServiceStatus = new(obj, obj.GetType().FullName);
    }

    public ServiceStatus ServiceStatus { get; }

}

public class ServiceTracker
{
    IEnumerable<IHostedService> HostedServices;

    public ServiceTracker(IEnumerable<IHostedService> hostedServices)
    {
        HostedServices = hostedServices;

        foreach (var service in HostedServices)
        {
            if (service is IProvidesServiceStatus providesServiceStatus)
            {
                SourceCache.AddOrUpdate(providesServiceStatus.ServiceStatus);
            }
            else
            {
                SourceCache.AddOrUpdate(new ServiceStatusAdapter(service).ServiceStatus);
            }
        }
    }

    public ISourceCache<ServiceStatus, string> SourceCache => sourceCache;
    private SourceCache<ServiceStatus, string> sourceCache = new SourceCache<ServiceStatus, string>(x => x.Key);
}
