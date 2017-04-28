using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public interface IReadHandle
       : IReferencable
        //, ITreeHandlePersistence  TODO
    {
        object Object { get; }
        bool HasObject { get; }
    }
}
