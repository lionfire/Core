using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Assets
{

    //public interface IHasHRAsset<out ConcreteType>
    //: IHasHRAsset
    //where ConcreteType : class
    //{
    //    new HRAsset<ConcreteType> HAsset
    //    {
    //        get;
    //        set;
    //    } // Add set?#endif
    //      //		ConcreteType AssetObject { get; } // Might as well expose this?
    //}
    public interface IHasHRAsset
    {
        IHRAsset<object> HAsset { get; set; }
        object AssetObject { get; }

    }
}
