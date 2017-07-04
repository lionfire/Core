using LionFire.Instantiating;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LionFire.Notifications.Abstractions
{

    


    public class TAppAlert : Notifier, ITemplate<AppAlert>
    {
    }

    class AppAlert : Notification
    {
    }
}
