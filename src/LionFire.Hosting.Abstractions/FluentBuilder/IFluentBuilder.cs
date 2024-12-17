using Microsoft.Extensions.Configuration;
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

    HashSet<string> RunMarkers { get; }
    T GetOrCreateSubBuilder<T>(Func<T> factory);
}

public abstract class HostApplicationSubBuilder : IHostApplicationSubBuilder
{
    #region SubBuilders

    Dictionary<Type, object> subBuilders = new();
    public T GetOrCreateSubBuilder<T>(Func<T> factory)
    {
        if (subBuilders.ContainsKey(typeof(T))) return (T)subBuilders[typeof(T)];
        var result = factory();
        if (result is { } notNullResult) subBuilders.Add(typeof(T), notNullResult);
        return result;
    }

    #endregion

    public HashSet<string> RunMarkers { get; } = new();
    public abstract IHostApplicationBuilder IHostApplicationBuilder { get; }

    #region Derived

    public IServiceCollection Services => IHostApplicationBuilder.Services;
    public IConfiguration Configuration => IHostApplicationBuilder.Configuration;
    public bool HasConfiguration => IHostApplicationBuilder?.Configuration != null;

    #endregion

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

    public static T ConfigureServices<T>(this T b, Action<IServiceCollection> configure)
        where T : IHostApplicationSubBuilder
    {
        configure(b.IHostApplicationBuilder.Services);
        return b;
    }

    public static T ThrowIfNotFirstTime<T>(this T b, [CallerMemberName] string? name = null)
        where T : IHostApplicationSubBuilder
    {
        ArgumentNullException.ThrowIfNull(name);

        if (b.RunMarkers.Contains(name))
        {
            throw new Exception("Cannot execute more than once: " + name);
        }
        else
        {
            b.RunMarkers.Add(name);
        }
        return b;
    }
    public static T IfFirstTime<T>(this T b, Action<T> configure, [CallerMemberName] string? name = null)
        where T : IHostApplicationSubBuilder
    {
        ArgumentNullException.ThrowIfNull(name);

        if (!b.RunMarkers.Contains(name))
        {
            configure(b);
            b.RunMarkers.Add(name);
        }
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