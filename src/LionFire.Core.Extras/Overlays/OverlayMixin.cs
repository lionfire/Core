
using System;
using System.Collections.Generic;
using System.Linq;
#if !NET35
#endif
using System.Reflection;
using System.Diagnostics.Contracts;

namespace LionFire.Overlays
{
    public class OverlayMixin<T> : IOverlay<T>
        where T : class, new()
    {
        public OverlayParameters Parameters { get { return parameters; } }
        private OverlayParameters parameters;

        // Alternative approach: consider assigning defaultInstance to the proxy class instance so that this isn't required.  But this is probably costly on a per-create basis with no significant gain.
        private T defaultInstance;
#if NET35
		public T DefaultInstance{get{return defaultInstance;}}
#endif
        #region Elements

        private const int DefaultOverlayCapacity = 6;
		private SortedList<int, KeyValuePair<string, T>> elements =  new SortedList<int, KeyValuePair<string, T>>(DefaultOverlayCapacity
#if AOT
           ,  Singleton<IntComparer>.Instance
#endif
            );

        int maxValue { get { return elements.Keys.LastOrDefault(); } }
        int minValue { get { return elements.Keys.FirstOrDefault(); } }

        #endregion

        public OverlayMixin(OverlayParameters parameters, T defaultInstance)
        {
            this.parameters = parameters;
            this.defaultInstance = defaultInstance;
        }

        #region Layers Methods

        #endregion

        public T Proxy
        {
            get
            {
                //IProxyTargetAccessor pta = (IProxyTargetAccessor)this;
                return this as T;
                //pta.DynProxyGetTarget()
                //return (T)pta.DynProxyGetTarget();
            }
        }

        public void Set(int priority, T overlayObject, string label = null)
        {
            if (parameters.DisallowAdd && priority >= maxValue) throw new InvalidOperationException("Addition has been disallowed (and specified priority is effectively an addition or replacement of the protected layer)");
            if (parameters.DisallowInsert && priority <= minValue) throw new InvalidOperationException("Addition has been disallowed (and specified priority is effectively an insertion or replacement of the protected layer)");

            if (elements.ContainsKey(priority))
            {
                var kvp = elements[priority];
                if (kvp.Key != label)
                {
                    throw new OverlayException("Can only replace overlay layers using the same label");
                }
                elements.Remove(priority);
            }
            elements.Add(priority, new KeyValuePair<string, T>(label, overlayObject));
        }

        public void Add(T overlayObject, string label = null)
        {
            if (parameters.DisallowAdd) throw new InvalidOperationException("Addition has been disallowed");
            elements.Add(maxValue + 1, new KeyValuePair<string, T>(label, overlayObject));
        }
        public void Insert(T overlayObject, string label = null)
        {
            if (parameters.DisallowInsert) throw new InvalidOperationException("Insertion has been disallowed");
            elements.Add(minValue - 1, new KeyValuePair<string, T>(label, overlayObject));
        }
        public void InsertBefore(T overlayObject, string label = null)
        {
            if (parameters.DisallowInsert) throw new InvalidOperationException("Insertion has been disallowed");
            elements.Add(minValue - 1, new KeyValuePair<string, T>(label, overlayObject));
        }
        #region Property Get / Set

        // Fasterflect???

        public void SetPropertyValue(string propertyName, object value, object[] index)
        {
            PropertyInfo pi = typeof(T).GetProperty(propertyName);

            T target;
            switch (parameters.SetTarget)
            {
                case OverlaySetTarget.Top:
                    if (elements.Count == 0) throw new OverlayException("Overlay is empty");
                    target = elements.LastOrDefault().Value.Value;
                    break;
                case OverlaySetTarget.Bottom:
                    if (elements.Count == 0) throw new OverlayException("Overlay is empty");
                    target = elements.FirstOrDefault().Value.Value;
                    break;
                case OverlaySetTarget.Default: // get value for
                case OverlaySetTarget.Disallowed:
                default:
                    throw new OverlayException("Setting properties has not been enabled for this overlay");
            }

            Contract.Assert(target != null);

            pi.SetValue(target, value, null);
        }

        public object GetPropertyValue(string propertyName, object[] args)
        {
            PropertyInfo pi = typeof(T).GetProperty(propertyName);

            // TODO: Allow user to specify a prototype object with unset (default) values (or use DefaultValue).
            // If, when getting, the value matches the unset value, revert to a lower precedence item.

            // TODO: Mergeable
            //  - Allow user to specify attributes on properties to specify they should be merged
            //  [OverlayMerge] public ISet<string> Tags { get; }.  The data type must be mergeable. (Maybe by a merge factory that supports IMergeHandler<ISet<string>> etc.

            T target;
            switch (parameters.Precedence)
            {
                default:
                case OverlayPrecedence.Default:
                case OverlayPrecedence.Top:
                    if (elements.Count == 0 && defaultInstance == null) throw new OverlayException("Overlay is empty and there is no default instance set");
                    target = elements.LastOrDefault().Value.Value;
                    break;
                case OverlayPrecedence.Bottom:
                    if (elements.Count == 0 && defaultInstance == null) throw new OverlayException("Overlay is empty and there is no default instance set");
                    target = elements.FirstOrDefault().Value.Value;
                    break;
            }

            if (target == null)
            {
                target = defaultInstance;
            }
            Contract.Assert(target != null);

            object result = pi.GetValue(target, args);
            return result;
        }

        #endregion

        #region Disposing

#if AOT
        public event EventHandler Disposing;
#else
        public event Action<IOverlay<T>> Disposing;
#endif

        public void Dispose()
        {
            var ev = Disposing;
            if (ev != null) ev(this);

            throw new NotImplementedException("TODO: Proper dispose");
        }

        #endregion

    }
}
