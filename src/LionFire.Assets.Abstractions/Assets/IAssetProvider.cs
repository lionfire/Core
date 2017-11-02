using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Handles;


namespace LionFire.Assets
{
    public interface INotifyingAssetProvider : INotifyPersistence, IAssetProvider { }

    public interface IAssetProvider
    {
        T Load<T>(string assetSubPath, object context = null) where T : class;
        void Save(string assetSubPath, object obj, object context = null);
#if TODO
        //Task<T> Load<T>(string assetSubPath, object context = null) where T : class;
        //Task Save(string assetSubPath, object obj, object context = null);
#endif

        Task<IEnumerable<string>> Find<T>(string searchString = null, object context = null);
    }

   
}
