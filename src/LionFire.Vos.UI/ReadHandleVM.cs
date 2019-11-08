using LionFire.Persistence;
using System;
using System.ComponentModel;

namespace LionFire.Vos.UI
{

    public class ReadHandleVM<T> : HandleVM<T>, INotifyPropertyChanged
    {

        #region ReadHandle

        [SetOnce]
        public IReadHandle<T> ReadHandle
        {
            get => readHandle;
            set
            {
                if (readHandle == value) return;
                if (readHandle != default) throw new AlreadySetException();
                readHandle = value;
                OnHandleChanged(value);
            }
        }
        private IReadHandle<T> readHandle;

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
        
    }
}
