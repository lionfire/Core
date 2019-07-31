using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public interface IHasHRAsset
    {
        IHRAsset HRAsset { get; set; }
        object AssetObject { get; }

    }
}
