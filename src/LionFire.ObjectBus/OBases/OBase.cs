﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
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