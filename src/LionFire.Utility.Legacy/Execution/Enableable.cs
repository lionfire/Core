using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace LionFire.Structures
{
    //public class SimpleEnableableBase
    //{
    //    public bool Enabled
    //    {
    //        get { return enabled; }
    //        set
    //        {
    //            if (enabled == value) return;

    //            if (value)
    //            {
    //                UpdateCanEnable(true);
    //                if (!CanEnable) throw new LionFireException("CanEnable is false: " + CannotEnableMessage);
    //            }

    //            _SetEnabled(value);

    //        }
    //    } protected bool enabled = false;
        
    //    public event Action<object, bool> EnabledChanged;

    //    protected virtual void OnEnabled()
    //    {

    //    }

    //    protected virtual void OnDisabled()
    //    {
    //    }
    //}

    public class EnableableBase : IEnableable, IPropertyChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Parameter is what it was changed to.  (TODO RENAME - EnabledChangedTo)
        /// </summary>
        public event Action<object, bool> EnabledChanged;

        /// <summary>
        /// Parameter is what it is changing to.  (TODO RENAME - EnabledChangingTo)
        /// </summary>
        public event Action<object, bool> EnabledChanging;

        #region Enabled

        public bool TrySetEnabled(bool newEnabled = true)
        {
            if (enabled == newEnabled) return true;

            UpdateCanEnable(true);
            if (newEnabled && !CanEnable) return false;

            _SetEnabled(newEnabled);

            return true;
        }

        [SerializeDefaultValue(false)]
        [Ignore]
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled == value) return;

                if (value)
                {
                    UpdateCanEnable(true);
                    if (!CanEnable) throw new LionFireException("CanEnable is false: " + CannotEnableMessage);
                }
                _SetEnabled(value);
            }
        }
        protected bool enabled = false;

        private void _SetEnabled(bool value)
        {
            var ev = EnabledChanging;
            if (ev != null) ev(this, value);

            enabled = value;

            if (enabled)
            {
                OnEnabled();
            }
            else
            {
                OnDisabled();
            }
            EnabledChanged?.Invoke(this, enabled);
        }

        #endregion

        protected virtual void OnEnabled()
        {
            Debug.WriteLine($"{this}: OnEnabled()");
        }

        protected virtual void OnDisabled()
        {
        }

        public bool CanEnable => canEnable;
        public bool canEnable => !CannotEnableReasons.ToArray().Any();

        public virtual bool DeferEnable => false;

        public IEnumerable<RuleError> CannotEnableReasons
        {
            get
            {
                if (cannotEnableReasons == null) { UpdateCanEnable(false); }
                return cannotEnableReasons;
            }
        } protected List<RuleError> cannotEnableReasons;

        public string CannotEnableMessage
        {
            get
            {
                if (cannotEnableReasons == null) { return ""; }
                return CannotEnableReasons.ToArray().Any() ? CannotEnableReasons.ToArray().Select(r => r.Message).Aggregate((x, y) => x.ToString() + System.Environment.NewLine + y.ToString()) : "";
            } // TODO - Consider wrapping with try/catch?
        }

#if AOT
		public event ActionBool CanEnableChanged;

//		[Synchronized]
		private event ActionBool MyHandlerZYX
		{
			add
			{
				myHandler = (ActionBool) Delegate.Combine(myHandler, value);
			}
			remove
			{
				myHandler = (ActionBool) Delegate.Remove(myHandler, value);
				
			}

		} private  ActionBool myHandler;

		private void AOT_Dummy()
		{
				ActionBool a = null;
				ActionBool b = null;
				System.Threading.Interlocked.CompareExchange<ActionBool>(ref a, b, null);
			CanEnableChanged += null;
			CanEnableChanged -= null;
		}
#else
        public event Action<bool> CanEnableChanged;
#endif
        public event Action<string> CannotEnableMessageChanged;
        public event Action CannotEnableReasonsChanged;

        public virtual void UpdateCanEnable(bool raiseEvents = true)
        {
            bool currentCanEnable = cannotEnableReasons == null ? false : canEnable;
            bool oldCanEnable = currentCanEnable;
            string oldCannotEnableMessage = CannotEnableMessage;

            CalculateCanEnable();

            if (raiseEvents)
            {
                if (oldCanEnable != currentCanEnable)
                {
                    OnPropertyChanged("CanEnable");
                    var ev = CanEnableChanged; if (ev != null) ev(canEnable);
                }

                var newCannotEnableMessage = CannotEnableMessage;

                if (oldCannotEnableMessage != newCannotEnableMessage)
                {
                    { var ev = CannotEnableReasonsChanged; if (ev != null) ev(); }
                    { var ev = CannotEnableMessageChanged; if (ev != null) ev(newCannotEnableMessage); }
                    //OnPropertyChanged("CannotEnableReasons");
                    //OnPropertyChanged("CannotEnableMessage"); // TODO REVIEW: Consider enabling this so remote clients can see why they can't enable things, such as modules. (But does it spam, during theatre creation?)
                }
            }
            //{ OLD
            //    var ev = CannotEnableReasonsChanged; if (ev != null) ev();
            //}
        }

        protected virtual bool CalculateCanEnable()
        {
            if (cannotEnableReasons == null)
            {
                cannotEnableReasons = new List<RuleError>();
            }
            else
            {
                cannotEnableReasons.Clear();
            }
            return true;
        }

        // FUTURE - add rules here, and have them validated by CanEnable.  This makes most sense.

        #region IPropertyChanged Implementation

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        } event PropertyChangedEventHandler propertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            {
                var ev = propertyChanged;
                if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
            }
            {
                var ev = PropertyValueChanged;
                if (ev != null) ev(propertyName);
            }
        }

        public event Action<string> PropertyValueChanged;

        #endregion
    }

    public class Enableable : EnableableBase, IEnableable
    {
        [Ignore]
        public Action EnableAction;
        [Ignore]
        public Action DisableAction;
        [Ignore]
        public Func<List<RuleError>> CalculateCannotEnableReasons;

        protected override void OnEnabled()
        {
            var d = EnableAction;
            if (d != null) d();
        }

        protected override void OnDisabled()
        {
            var d = EnableAction;
            if (d != null) d();
        }

        protected override bool CalculateCanEnable()
        {
            if (!base.CalculateCanEnable()) return false;

            var d = CalculateCannotEnableReasons;
            if (d != null)
            {
                var result = d();
                if (result != null)
                {
                    cannotEnableReasons.AddRange(result);
                }
            }
            return true;
        }

        // FUTURE - Event for CanEnableChanged
        // FUTURE - Event for CannotEnableReasons changed
        // FUTURE - add rules here, and have them validated by CanEnable.  This makes most sense.

    }
}
