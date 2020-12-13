using System;
using System.ComponentModel;

namespace LionFire.UI
{
    public class UIObject : IUIObject
    {
        #region Parent

        public IUIObject Parent
        {
            get => parent;
            set
            {
                if (parent == value) return;
                if (value != null && parent != default(IUIObject)) throw new NotSupportedException("Parent can only be set once or back to null.");
                parent = value;
            }
        }
        private IUIObject parent;

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
