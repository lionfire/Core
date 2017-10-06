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
using LionFire.MultiTyping;

namespace LionFire.Assets
{

    public static class AssetProviderExtensions
    {
        public static IEnumerable<string> Find<T>(this string searchString)
        {
            return AssetProvider.Find<T>(searchString);
        }

        private static IAssetProvider AssetProvider => InjectionContext.Current.GetService<IAssetProvider>();

        // TODO: Async?
        //public static Task<T> LoadAsync<T>(this string assetSubPath, object context = null)
        //{
        //}

        public static ReturnType Load<ReturnType>(this string assetSubPath, Type assetType, object context = null)
        {
            return (ReturnType)typeof(AssetProviderExtensions).GetMethod("Load", new Type[] { typeof(string), typeof(object) }).MakeGenericMethod(assetType).Invoke(null, new object[] { assetSubPath, context });
        }
        public static T Load<T>(this string assetSubPath, object context = null)
            where T : class
        {
            var result = AssetProvider.Load<T>(assetSubPath);
            ValidateIfNeeded(result, context.ObjectAsType<InstantiationContext>());
            return result;
        }
        public static T Load<T>(this string assetSubPath, string concreteTypeName, object context = null)
        {
            Type t = TypeResolver.Default.Resolve(concreteTypeName); // TODO: Get typeresolver from InstantiationContext

            var mi = typeof(IAssetProvider).GetMethod("Load", BindingFlags.Instance | BindingFlags.Public).MakeGenericMethod(t);

            var result = (T)mi.Invoke(AssetProvider, new object[] { assetSubPath });
            ValidateIfNeeded(result, context.ObjectAsType<InstantiationContext>());
            return result;
        }

        public static object Load(this string assetSubPath, string concreteTypeName, object context = null)
        {
            Type t = TypeResolver.Default.Resolve(concreteTypeName); // TODO: Get typeresolver from InstantiationContext

            var mi = typeof(IAssetProvider).GetMethod("Load", BindingFlags.Instance | BindingFlags.Public).MakeGenericMethod(t);
            var result = mi.Invoke(AssetProvider, new object[] { assetSubPath });
            ValidateIfNeeded(result, context.ObjectAsType<InstantiationContext>());
            return result;
        }

        private static void ValidateIfNeeded(object result, InstantiationContext instantiationContext)
        {
            if (result is IValidatable v && (instantiationContext == null || !instantiationContext.Loading.SkipValidation))
            {
                v.Validate(new ValidationContext(result, ValidationKind.Deserialized));
            }
        }

        //public static void Save<T>(this string assetSubPath, T obj)
        //{
        //    var ap = (IAssetProvider)ManualSingleton<IServiceProvider>.Instance.GetService(typeof(IAssetProvider));
        //     ap.Save<T>(assetSubPath, obj);
        //}

        public static void Save<T>(this T obj, string assetSubPath, PersistenceContext persistenceContext = null)
        {
            (obj as INotifyOnSaving)?.OnSaving();

            object inst;
            if (persistenceContext==null||persistenceContext.AllowInstantiator != false)
            {
                inst = obj.ToInstantiatorOrObject(); // FUTURE - Use instantiation framework?
            }
            else
            {
                inst = obj;
            }

            if (persistenceContext == null)
            {
                persistenceContext = new PersistenceContext();
            }
            persistenceContext.RootObject = obj;

            AssetProvider.Save(assetSubPath, inst, persistenceContext);
        }

    }
}
