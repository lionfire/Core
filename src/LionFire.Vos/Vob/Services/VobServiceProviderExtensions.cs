﻿#nullable enable
using System;
using LionFire.Dependencies;
using LionFire.DependencyInjection;
using LionFire.FlexObjects;
using LionFire.Vos;
using LionFire.Vos.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public class FlexSingletonRegistration
{
}

public static class FlexActivatorX
{
    public static bool TryCreateInstance<T>(this IFlex flex, out T? result)
    {
        var sp = flex.Query<IServiceProvider>();
        if (sp == null) { result = default; return false; }

        result = ActivatorUtilities.CreateInstance<T>(sp);
        return result != null;
    }
}


public class FlexAsServiceProvider : IServiceProvider
{
    private IFlex flex;

    public FlexAsServiceProvider(IFlex flex)
    {
        this.flex = flex;
    }

    public object GetService(Type serviceType)
    {
        throw new NotImplementedException();
    }
}

public static class VobServiceProviderExtensions
{
    public static IVob AddDynamicServiceProvider(this IVob vob, Action<IServiceCollection>? configurator = null, IServiceProvider parentServiceProvider = null)
    {
        IServiceProvider? existingServiceProvider;

        #region DEPRECATED: VobNode

        var vobI = (IVobInternals)vob;

        var existingNode = vobI.TryAcquireOwnVobNode<IServiceProvider>();
        //if (existingServiceProvider != null)
        //{
        //    throw new AlreadySetException($"{nameof(vob)}.TryAcquireOwnVobNode<IServiceProvider>()?.Value != null");
        //}

        #endregion

        existingServiceProvider = vob.Query<IServiceProvider>();

        var dsp = new DynamicServiceProvider(parentServiceProvider ?? existingServiceProvider);
        configurator?.Invoke(dsp);

        vob.AddOrReplace<IServiceProvider>(dsp); // Typically: replacing the application-wide IServiceProvider here
        vob.AddSingle<IServiceCollection>(dsp);
        //vob.AddSingle<DynamicServiceProvider>(dsp);

        vob.TryAddOwn<IServiceProvider>(_ => dsp);

        return vob;
    }

#if UNUSED
    public static IVob AddDynamicServiceProvider2(this IVob vob, Action<DynamicServiceProvider2> configurator = null, IServiceProvider parentServiceProvider = null, bool allowDiscardExistingServiceProvider = false)
    {
        var dsp = new DynamicServiceProvider2(parentServiceProvider);
        //dsp.ServiceProvidersFunc = () => vob.QueryAll<IServiceProvider>().Where(sp => !ReferenceEquals(sp, dsp));

        configurator(dsp);

        vob.AddSingle<DynamicServiceProvider2>(dsp);
        vob.Add<IServiceProvider>(dsp);

        return vob;
    }

    public static IVob AddServiceProvider_OLD(this IVob vob, Action<IServiceCollection> configurator = null, IServiceProvider parentServiceProvider = null, bool allowDiscardExistingServiceProvider = false)
    {        
        var dsp = new DynamicServiceProvider();
        configurator?.Invoke(dsp);

        var vobI = vob as IVobInternals;
        vobI.GetOrAddVobNode<IServiceProvider>(dsp); // DEPRECATED: VobNode
        vobI.GetOrAddVobNode<IServiceCollection>(dsp); // DEPRECATED: VobNode


        var existingServiceProvider = vobI.TryAcquireOwnVobNode<IServiceProvider>()?.Value;
        //vob.AcquireOrAddNextVobNode<IServiceProvider, VobServiceProvider>(addAtRoot: false);
        if (parentServiceProvider != null && existingServiceProvider != null && !ReferenceEquals(parentServiceProvider, existingServiceProvider) && !allowDiscardExistingServiceProvider)
        {
            throw new ArgumentException($"{nameof(parentServiceProvider)} != null && {nameof(vob)}.TryAcquireOwnVobNode<IServiceProvider>()?.Value != null && !{nameof(allowDiscardExistingServiceProvider)}");
        }
        dsp.Parent = parentServiceProvider ?? existingServiceProvider;
        return vob;
    }
#endif
}

