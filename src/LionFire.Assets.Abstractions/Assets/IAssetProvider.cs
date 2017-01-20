using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public interface IAssetProvider
    {
        T Load<T>(string assetSubPath);
        void Save(string assetSubPath, object obj);

        IEnumerable<string> Find<T>(string searchString);
    }
}
