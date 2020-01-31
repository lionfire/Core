using LionFire.Ontology;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public static class IOBaseExtensions
    {
        public static IOBase TryGetOBase(this IReference reference) => 
            (reference.GetReadHandle<object>() as IHas<IOBase>)?.Object ?? 
            (reference.ToReadWriteHandle<object>() as IHas<IOBase>)?.Object;

        public static Task<IEnumerable<string>> List(this IOBase obase, IReference parent, Type type) 
            => (Task<IEnumerable<string>>)obase.GetType().GetMethod("List", new Type[] { typeof(IReference) }).MakeGenericMethod(type).Invoke(obase, new object[] { parent }); // TOOPTIMIZE - cache the MethodInfo?

    }


    // REVIEW
    //public interface IRest
    //{
    //    object Get(string uri);
    //    void Delete(string uri);
    //    void Put(string uri, object data);
    //    object Post(string uri, object data);
    //}

    //public class OBase
    //{

    //    //private static Dictionary<string, IOBase> objectStoreProvidersByUriScheme = new Dictionary<string, IOBase>();

    //    //private static void RegisterType<T>() where T : IOBase, new()
    //    //{
    //    //    var objectStore = new T();
    //    //    foreach (var scheme in objectStore.UriSchemes)
    //    //    {
    //    //        objectStoreProvidersByUriScheme.Add(scheme, objectStore);
    //    //    }
    //    //}

    //    //public static IOBase GetObjectStore(IReference reference)
    //    //{
    //    //    if (!objectStoreProvidersByUriScheme.ContainsKey(reference.Scheme)) throw new ArgumentException("Unknown scheme type: " + reference.Scheme);
    //    //    return objectStoreProvidersByUriScheme[reference.Scheme];
    //    //}

    //    //static OBase()
    //    //{
    //    //    RegisterType<FileObjectStore>();

    //    //    var fileObjectStore = new FileOBase();
    //    //    foreach (var scheme in fileObjectStore.UriSchemes)
    //    //    {
    //    //        objectStoreProvidersByUriScheme.Add(scheme, fileObjectStore);
    //    //    }
    //    //}

    //    #region Load

    //    public static object Load(string uri)
    //    {
    //        return Load(uri.ToReference());
    //    }

    //    #endregion

    //    #region Save

    //    public static void Save(string uri, object obj)
    //    {
    //        Save(uri.ToReference(), obj);
    //    }

    //    #endregion

    //}    
}
