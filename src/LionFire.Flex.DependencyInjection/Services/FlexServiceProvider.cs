using LionFire.DependencyInjection;

namespace LionFire.FlexObjects.Services;

public enum ServicesFromFlex
{
    Disabled = 0,
    Self = 1,
    SelfAndAncestors = 2,
}




/// <summary>
/// 
/// </summary>
/// <remarks>
/// Register ITypedObjectProvider to extend IFlex via the embedded DynamicServiceProvider
/// </remarks>
public class FlexServiceProvider : IServiceProvider, ITypedObjectProvider
{
    #region Relationships

    public IFlex Flex { get; }

    #endregion

    #region Configuration

    public ServicesFromFlex ServicesFromFlex { get; set; } = ServicesFromFlex.Self;

    #endregion

    #region Parameters

    #region Fallback Parent IServiceProvider

    public IServiceProvider? Parent
    {
        get => parent ??= ParentFunc?.Invoke();
        set => parent = value;
    }
    private IServiceProvider? parent;
    public Func<IServiceProvider?>? ParentFunc { get; set; } = null;

    #endregion

    #endregion

    #region Lifecycle

    public FlexServiceProvider(IFlex flex, IServiceProvider fallbackServiceProvider)
    {
        Flex = flex;
        DynamicServiceProvider = new DynamicServiceProvider(fallbackServiceProvider);
    }
    public FlexServiceProvider(IFlex flex, Func<FlexServiceProvider, IServiceProvider?>? fallback = null)
    {
        Flex = flex;

        var injectionStrategy = fallback ?? DefaultStrategy;
        DynamicServiceProvider = new DynamicServiceProvider(() => injectionStrategy(this));
    }

    #endregion

    #region State

    public DynamicServiceProvider DynamicServiceProvider { get; }

    #region (static) Strategies: inject fallback IServiceProvider into this

    public static Func<FlexServiceProvider, IServiceProvider?> DefaultStrategy { get; set; } = OneShotServiceProviderFromFlex;

    public static IServiceProvider? OneShotServiceProviderFromFlex(FlexServiceProvider @this)
    {
        var result = @this.Flex.Query<IServiceProvider>();
        if (result != null)
        {
            @this.Parent = result;
            @this.ParentFunc = null;
        }
        return result;
    }

    public static IServiceProvider? FallbackServiceProviderFromFlex(FlexServiceProvider @this)
        => @this.Flex.Query<IServiceProvider>();

    public static IServiceProvider? FallbackServiceProviderFromFlexRecursive(FlexServiceProvider @this)
        => @this.Flex.RecursiveQuery<IServiceProvider>();

    #endregion

    #region (static) Strategies: inject fallback IServiceProvider into DynamicServiceProvider

    public static Func<FlexServiceProvider, IServiceProvider?> DefaultInjectionStrategy { get; set; } = OneShotServiceProviderFromFlex;

    public static IServiceProvider? InjectOneShotServiceProviderFromFlex(FlexServiceProvider @this)
    {
        var result = @this.Flex.Query<IServiceProvider>();
        if (result != null)
        {
            @this.DynamicServiceProvider.Parent = result;
            @this.DynamicServiceProvider.ParentFunc = null;
        }
        return result;
    }

    //public static IServiceProvider? InjectFallbackServiceProviderFromFlex(FlexServiceProvider @this)
    //    => @this.Flex.Query<IServiceProvider>();

    //public static IServiceProvider? InjectFallbackServiceProviderFromFlexRecursive(FlexServiceProvider @this)
    //    => @this.Flex.RecursiveQuery<IServiceProvider>();

    #endregion

    #endregion

    #region IServiceProvider: Pass-thru

    public object? GetService(Type serviceType)
    {
        object? result = null;

        if (ServicesFromFlex == ServicesFromFlex.Self)
        {
            result = Flex.Query(serviceType);
        }
        else if (ServicesFromFlex == ServicesFromFlex.SelfAndAncestors)
        {
            result = Flex.RecursiveQuery(serviceType);
        }

        result ??= DynamicServiceProvider.GetService(serviceType);

        return result;
    }

    #endregion

    public T? Query<T>(string? key = null)
    {
        T? result = default;
        if (key == null)
        {
            result = (T?)DynamicServiceProvider.GetService(typeof(T));
        }
        return result;
    }
    public object? Query(Type type, string? key = null)
    {
        object? result = null;
        if (key == null)
        {
            result = DynamicServiceProvider.GetService(type);
        }
        return result;
    }
}
