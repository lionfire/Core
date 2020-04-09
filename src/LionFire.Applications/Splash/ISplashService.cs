using System;

namespace LionFire.Applications.Splash
{
    // TODO: Add progress bar?

    public interface ISplashService
    {
        string Message { get; }
        IDisposable SetMessage(string message);

        event Action<string> MessageChanged;
    }
}
