using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Runtime.CompilerServices;

namespace LionFire.Hosting;

public interface IFluentBuilder
{
    // IHostApplicationBuilder IHostApplicationBuilder {get;}
}
public interface IHostApplicationSubBuilder
{
    IHostApplicationBuilder IHostApplicationBuilder { get; }
}

public static class IHostApplicationSubBuilderX
{
    public static T Host<T>(this T b, Action<IHostApplicationBuilder> configureDelegate)
        where T : IHostApplicationSubBuilder
        => b.ForIHostApplicationBuilder(configureDelegate);

    public static T ForIHostApplicationBuilder<T>(this T b, Action<IHostApplicationBuilder> configureDelegate)
        where T : IHostApplicationSubBuilder
    {
        configureDelegate(b.IHostApplicationBuilder);
        return b;
    }
}


// REVIEW - IFluentServiceBuilder - either get Services on IHostBuilder or deprecate IHostBuilder and throw NIE, and add this to ILionFireHostBuilder
// TODO - scrap this idea, and put IHostApplicationBuilder {get;} on IFluentBuilder, rename it to IHostApplicationSubBuilder
#if OLD
public interface IFluentServiceBuilder : IServiceCollection, IFluentBuilder
{ 
    IServiceCollection Services { get; }

    ServiceDescriptor this[int index] { get => ((IList<ServiceDescriptor>)Services)[index]; set => ((IList<ServiceDescriptor>)Services)[index] = value; }

    new int Count => Services.Count;

    new bool IsReadOnly => Services.IsReadOnly;

    new void Add(ServiceDescriptor item)
    {
        Services.Add(item);
    }

    new void Clear()
    {
        Services.Clear();
    }

    bool Contains(ServiceDescriptor item)
    {
        return Services.Contains(item);
    }

    void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
        Services.CopyTo(array, arrayIndex);
    }

    IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        return Services.GetEnumerator();
    }

    int IndexOf(ServiceDescriptor item)
    {
        return Services.IndexOf(item);
    }

    void Insert(int index, ServiceDescriptor item)
    {
        Services.Insert(index, item);
    }

    bool Remove(ServiceDescriptor item)
    {
        return Services.Remove(item);
    }

    void RemoveAt(int index)
    {
        Services.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Services).GetEnumerator();
    }
}

#endif