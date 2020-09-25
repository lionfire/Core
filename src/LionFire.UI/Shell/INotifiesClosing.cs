using System;

namespace LionFire.Shell
{
    public interface INotifiesClosing
    {
        event Action Closing; // Make cancelable?
        event Action Closed;
    }

}
