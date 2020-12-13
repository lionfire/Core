using System;
using System.Threading.Tasks;

namespace LionFire.UI
{
    public interface ICloseable
    {
        Task Close(bool force = false);
        event Action<object> Closed;
        
    }
}
