using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{
    public interface IMultiTyped : IReadonlyMultiTyped
    {
        void SetType<T>(T obj) where T : class;
    }

}
