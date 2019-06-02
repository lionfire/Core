		using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace LionFire.Events
{

 
    public abstract class PropertyChangedListenerBase<TargetInterfaceType>
        where TargetInterfaceType : class
    {
        public virtual bool ThrowOnUnsupported { get { return false; } }
        public virtual bool AttachOnSetTarget { get { return true; } }

        public bool IsTargetSupported
        {
            get { return this.NotifyingTarget != null; }
        }

        protected abstract void Attach();
        protected abstract void Detach();

        public bool IsAttached
        {
            get { return isAttached; }
            set
            {
                if (isAttached == value) return;

                if (value)
                {
                    if (target == null) throw new InvalidOperationException("No target.  Cannot attach");
                    if (!IsTargetSupported)
                    {
                        if (ThrowOnUnsupported)
                        {
                            throw new ArgumentException("Cannot attach to specified target. It is not supported.");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        Attach();
                    }
                }
                else
                {
                    Detach();
                }
                isAttached = value;
            }
        } private bool isAttached;

        protected TargetInterfaceType NotifyingTarget
        {
            get
            {
                return Target as TargetInterfaceType;
            }
        }

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
                    IsAttached = false;
                }

                target = value;
                if (target != null)
                {
                    targetType = target.GetType();

                    if (AttachOnSetTarget)
                    {
                        IsAttached = true;
                    }
                }
                else
                {
                    targetType = null;
                }
            }
        } private object target;

        public Type TargetType
        {
            get { return targetType; }
            private set
            {
                if (targetType == value) return;
                targetType = value;
                OnTargetTypeChanged();
            }
        } private Type targetType;

        protected virtual void OnTargetTypeChanged()
        {
        }

        #region (Protected) Event handling

        protected void OnPropertyNameChanged(string obj)
        {
            if (Target == null)
            {
                l.Error("Got property changed event '" + obj + "' but Target is null");
                return;
            }
            if (TargetType == null)
            {
                l.Error("Got property changed event '" + obj + "' but TargetType is null");
                return;
            }

            var pi = TargetType.GetProperty(obj);
            if (pi == null)
            {
                l.Trace("(Ignoring PropertyChanged event) Got property changed event for property name '" + obj + "' but property was not found on type: " + TargetType);
                return;
            }

            {
                var ev = this.PropertyChanged;
                if (ev != null)
                {
                    ev(pi);
                }
            }
            {
                var ev = this.PropertyChangedTo;
                if (ev != null)
                {
                    ev(pi, pi.GetValue(Target, null));
//                    ev(pi, pi.Get(Target));
                }
            }
        }

        #endregion

        #region (Public) Events

        public event Action<PropertyInfo> PropertyChanged;
        public event Action<PropertyInfo, object> PropertyChangedTo;

        #endregion

        private static readonly ILogger l = Log.Get();
    }
}
