using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Serialization
{
    public interface INotifyDeserialized
    {
        //void OnSerializing();
        //void OnSerialized();
        void OnDeserialized();
    }
}
