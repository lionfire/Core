using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public interface IAssetProvider
    {
        T Load<T>(string assetSubPath, PersistenceContext context = null) where T : class;
        void Save(string assetSubPath, object obj, PersistenceContext context = null);

        IEnumerable<string> Find<T>(string searchString = null, PersistenceContext context = null);
    }
}
