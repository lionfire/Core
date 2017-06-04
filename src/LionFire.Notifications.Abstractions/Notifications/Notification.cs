using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LionFire.Notifications
{

    public class Notification : INotifyPropertyChanged
    {

        #region NotificationStateFlags

        public NotificationStateFlags StateFlags
        {
            get { return stateFlags; }
            set
            {
                if (stateFlags == value) return;
                stateFlags = value;
                OnPropertyChanged(nameof(StateFlags));
            }
        }
        private NotificationStateFlags stateFlags;

        #endregion


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


    }
}
