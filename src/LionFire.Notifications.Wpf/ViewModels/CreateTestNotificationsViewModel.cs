using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Notifications.UI;

public class CreateTestNotificationsViewModel : Screen
{
    #region Message

    public string Message
    {
        get { return message; }
        set
        {
            if (message == value) return;
            message = value;
            NotifyOfPropertyChange(() => Message);
        }
    }
    private string message;

    #endregion

    #region Profile

    public string Profile
    {
        get { return profile; }
        set
        {
            if (profile == value) return;
            profile = value;
            NotifyOfPropertyChange(() => Profile);
        }
    }
    private string profile = "G3";

    #endregion


    #region Importance

    public Importance Importance
    {
        get { return importance; }
        set
        {
            if (importance == value) return;
            importance = value;
            NotifyOfPropertyChange(() => Importance);
        }
    }
    private Importance importance;

    #endregion

    #region Urgency

    public Urgency Urgency
    {
        get { return urgency; }
        set
        {
            if (urgency == value) return;
            urgency = value;
            NotifyOfPropertyChange(() => Urgency);
        }
    }
    private Urgency urgency;

    #endregion


    public void RaiseNotification()
    {
        Debug.WriteLine("TODO -RaiseNotification ");
    }
}
