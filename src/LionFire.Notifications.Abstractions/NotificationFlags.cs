using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Notifications
{
    public enum NotificationFlags
    {
        None = 0,
        MustAck = 1 << 0,
        Expires = 1 << 1,
    }

    
}
