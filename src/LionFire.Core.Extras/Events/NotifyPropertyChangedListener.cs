using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace LionFire.Events
{

    public class NotifyPropertyChangedListener : PropertyChangedListenerBase<INotifyPropertyChanged>, IPropertyChangedListener
    {

        protected override void Attach()
        {
            NotifyingTarget.PropertyChanged += new PropertyChangedEventHandler(NotifyingTarget_PropertyChanged);
        }

        protected override void Detach()
        {
            NotifyingTarget.PropertyChanged -= new PropertyChangedEventHandler(NotifyingTarget_PropertyChanged);
        }

        void NotifyingTarget_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyNameChanged(e.PropertyName);
        }

        //public event Action<object> ChangedFrom { add { } remove { } }
        //public event Action<object, object> ChangedFromTo { add { } remove { } }

        public bool HasFastChangedTo
        {
            get { return false; }
        }

        public bool HasChangedFrom
        {
            get { return false; }
        }

    }

}
