using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Events
{
    public class ObjectPropertyChangedListener
    {
        #region (Static) Dependency Acquisition

        static Type[] PropertyChangedListenerTypes;

        static ObjectPropertyChangedListener()
        {
            PropertyChangedListenerTypes = new[]
            {
                typeof(PropertyChangedListener),
                typeof(NotifyPropertyChangedListener),
            };
        }

        #endregion

        public object Target
        {
            get
            {
                return target;
            }
            set
            {
                if (target == value) return;

                if (target != null)
                {
                    foreach (var listener in Listeners)
                    {
                        if (listener.HasFastChangedTo)
                        {
                            listener.PropertyChangedTo -= OnPropertyChangedTo;
                        }
                        else
                        {
                            listener.PropertyChanged -= OnPropertyChanged;
                        }
                    }
                }

                target = value;

                if (target != null)
                {
                    Listeners.Clear();
                    foreach (Type type in PropertyChangedListenerTypes)
                    {
                        var listener = (IPropertyChangedListener)Activator.CreateInstance(type);

                        listener.Target = target;
                        if (listener.IsTargetSupported)
                        {
                            Listeners.Add(listener);
                        }
                    }
                    foreach (var listener in Listeners)
                    {
                        if (listener.HasFastChangedTo)
                        {
                            listener.PropertyChangedTo += OnPropertyChangedTo;
                        }
                        else
                        {
                            listener.PropertyChanged += OnPropertyChanged;
                        }
                    }
                }
            }
        }
        private object target;

        void OnPropertyChangedTo(PropertyInfo pi, object newValue)
        {
            {
                var ev = PropertyChanged;
                if (ev != null) ev(pi);
            }
            {
                var ev = PropertyChangedTo;
                if (ev != null) ev(pi, newValue);
            }
        }
        void OnPropertyChanged(PropertyInfo pi)
        {
            {
                var ev = PropertyChanged;
                if (ev != null) ev(pi);
            }
            {
                var ev = PropertyChangedTo;
                // REVIEW - Why did this Get work on MSVC / .NET 4?
                //                if (ev != null) ev(pi, pi.Get(Target));
                if (ev != null) ev(pi, pi.GetValue(Target, null));
            }
        }

        public event Action<PropertyInfo> PropertyChanged;
        public event Action<PropertyInfo, object> PropertyChangedTo;

        List<IPropertyChangedListener> Listeners = new List<IPropertyChangedListener>();

        public bool IsAttached
        {
            get
            {
                return Listeners.Count > 0;
            }
        }

        public bool HasFastChangedTo
        {
            get
            {
                foreach (var listener in Listeners)
                {
                    if (listener.HasFastChangedTo) return true;
                }
                return false;
            }
        }
    }

}
