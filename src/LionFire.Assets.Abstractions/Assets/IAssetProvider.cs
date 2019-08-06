using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LionFire.Assets
{
    public interface INotifyingAssetProvider : INotifyPersistence, IAssetProvider { }

    public interface IAssetProvider
    {
        
        Task<T> Load<T>(string assetSubPath) where T : class;
        Task Save(string assetSubPath, object obj);
#if TODO
        //Task<T> Load<T>(string assetSubPath, object context = null) where T : class;
        //Task Save(string assetSubPath, object obj, object context = null);
#endif

        Task<IEnumerable<string>> Find<T>(string searchString = null);
    }
}
