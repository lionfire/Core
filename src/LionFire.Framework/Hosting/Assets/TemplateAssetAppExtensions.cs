using LionFire.Assets;
using LionFire.Structures;
using LionFire.Instantiating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Applications.Hosting
{
    public static class TemplateAssetAppExtensions
    {
        public static IAppHost Add<T, TInstance>(this IAppHost host, string assetSubpath)
            where T : class, ITemplate
        {
            return host.Add<T,object>(assetSubpath,null);
        }

        [Blocking]
        public static IAppHost Add<T,TInstance>(this IAppHost host, string assetSubpath, Action<TInstance> initializer)
            where T : class, ITemplate
        {
            throw new NotImplementedException("TODO: IAssetProvider");
            //var sp = ManualSingleton<IServiceProvider>.Instance;
            //if (sp == null)
            //{
            //    throw new InvalidOperationException("ManualSingleton<IServiceProvider>.Instance must be set.  Application must be bootstrapped or initialized before invoking this method.");
            //}

            //var ap = (IAssetProvider)sp.GetService(typeof(IAssetProvider));
            //if (ap == null)
            //{
            //    throw new InvalidOperationException("No IAssetProvider is registered.  One must be registered in order to retrieve the asset using this method.");
            //}

            //var template = ap.Load<TValue>(assetSubpath).Result;

            //if (template == null) throw new ArgumentException($"Failed to load: \"{assetSubpath}\" of type '{typeof(TValue).FullName}'");

            //var templateInstance = template.Create();

            //if (initializer != null) { initializer((TInstance)templateInstance); }

            ////var appComponent = templateInstance as IAppComponent;

            //    //if (appComponent == null)
            //    //{
            //    //    throw new ArgumentException($"Created object of type '{templateInstance.GetType().FullName}' does not implement IAppComponent.  There is no implementation available for adding it to the IAppHost.");
            //    //}

            //    host.Add(templateInstance);

            //return host;
        }

    }

}
