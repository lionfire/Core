using System;
using System.ComponentModel;

namespace LionFire.Applications.Splash
{
    // TODO: Add progress bar?

    public interface ISplashService : INotifyPropertyChanged
    {
        string Message { get; }
        IDisposable SetMessage(string message);

        event Action<string> MessageChanged;

        double Progress{get;set;}
    }
}
