using Caliburn.Micro;
using LionFire.Threading;
using System;

namespace LionFire.UI
{
    public interface IUIPlatform
    {
        IDispatcher Dispatcher { get; }
        
        IEventAggregator EventAggregator { get; }
        
        bool IsViewType(Type type);
        bool IsPlatformType(Type type);
    }
}
