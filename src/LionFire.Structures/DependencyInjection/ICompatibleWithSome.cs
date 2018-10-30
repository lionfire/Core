using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.DependencyInjection
{
    public interface ICompatibleWithSome<T>
    {
        bool IsCompatibleWith(T obj);
    }
}
