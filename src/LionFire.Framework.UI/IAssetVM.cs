using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LionFire.ObjectBus;
using LionFire.Assets;

namespace LionFire.Shell
{
    
    public interface IAssetVM 
        //: IHasHandle
    {
        IHAsset HAsset { get; }

        void Save();
        void Delete();
        ICommand DeleteCommand { get; }

        string DisplayName { get;  }
    }
}
