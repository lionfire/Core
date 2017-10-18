using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public interface IAssetProvider
    {
        T Load<T>(string assetSubPath, object context = null) where T : class;
        void Save(string assetSubPath, object obj, object context = null);

        IEnumerable<string> Find<T>(string searchString = null, object context = null);
    }
}
