
using System;

namespace LionFire.UI
{
    public interface IDispatcher
    {
        bool CheckAccess();
        void Invoke(Action p);
    }
}
