using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LionFire.Structures
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
