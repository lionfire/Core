using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Structures;
using System.Threading.Tasks;
using LionFire.Validation;
using LionFire.Types;
using System.Reflection;
using LionFire.Instantiating;
using LionFire.Persistence;
using LionFire.DependencyInjection;

namespace LionFire.Assets
{

    // TODO: Async?
    public static class AssetProviderExtensions
    {
        public static IEnumerable<string> Find<T>(this string searchString)
        {
            return AssetProvider.Find<T>(searchString);
        }

        private static IAssetProvider AssetProvider => InjectionContext.Current.GetService<IAssetProvider>();

        public static T Load<T>(this string assetSubPath, InstantiationContext context = null)
            where T : class
        {
            var result = AssetProvider.Load<T>(assetSubPath);
            var v = result as IValidatable;
            if (v != null && (context == null || !context.Loading.SkipValidation)) { v.Validate(new ValidationContext(result, ValidationKind.Deserialized)); }
            return result;
        }
        public static T Load<T>(this string assetSubPath, string concreteTypeName, InstantiationContext context = null)
        {
            Type t = TypeResolver.Default.Resolve(concreteTypeName); // TODO: Get typeresolver from InstantiationContext

            var mi = typeof(IAssetProvider).GetMethod("Load", BindingFlags.Instance | BindingFlags.Public).MakeGenericMethod(t);

            var result = (T)mi.Invoke(AssetProvider, new object[] { assetSubPath });
            var v = result as IValidatable;
            if (v != null && (context == null || !context.Loading.SkipValidation)) { v.Validate(new ValidationContext(result, ValidationKind.Deserialized)); }
            return result;
        }

        public static object Load(this string assetSubPath, string concreteTypeName, InstantiationContext context = null)
        {
            Type t = TypeResolver.Default.Resolve(concreteTypeName); // TODO: Get typeresolver from InstantiationContext

            var mi = typeof(IAssetProvider).GetMethod("Load", BindingFlags.Instance | BindingFlags.Public).MakeGenericMethod(t);

            var result = mi.Invoke(AssetProvider, new object[] { assetSubPath });
            if (result is IValidatable v && (context == null || !context.Loading.SkipValidation)) { v.Validate(new ValidationContext(result, ValidationKind.Deserialized)); }

            return result;
        }

        //public static void Save<T>(this string assetSubPath, T obj)
        //{
        //    var ap = (IAssetProvider)ManualSingleton<IServiceProvider>.Instance.GetService(typeof(IAssetProvider));
        //     ap.Save<T>(assetSubPath, obj);
        //}

        public static void Save<T>(this T obj, string assetSubPath, PersistenceContext persistenceContext = null)
        {
            (obj as INotifyOnSaving)?.OnSaving();

            var inst = obj.ToInstantiatorOrObject(); // FUTURE - Use instantiation framework?

            if (persistenceContext == null)
            {
                persistenceContext = new PersistenceContext();
            }
            persistenceContext.RootObject = obj;

            AssetProvider.Save(assetSubPath, inst, persistenceContext);
        }

    }
}
