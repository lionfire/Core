using System;
using System.Threading.Tasks;

namespace LionFire.UI
{
    public interface ILoadable
    {
        bool CanLoad { get; }
        Task Load();
    }

    public interface ILoaded<out TFault> : ILoaded
    {
         TFault Fault { get; }
         event Action<object, TFault> Faulted;
        event Action<object> Loaded;
    }

    public interface ILoaded 
    {
        bool? IsLoaded { get; }
        bool IsLoading { get; }

         bool IsFaulted { get; }

         //object Fault { get; }

         
    }
}
