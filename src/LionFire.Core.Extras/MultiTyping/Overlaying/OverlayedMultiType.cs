
using LionFire.ExtensionMethods;
using LionFire.Overlays;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.MultiTyping.Overlaying
{
    public class OverlayedMultiType : IMultiTyped
    {
        #region Default Layer

        #region DefaultLayerPriority

        public int DefaultLayerPriority
        {
            get => defaultLayerPriority;
            set
            {
                if (defaultLayerPriority == value) return;
                int oldPriority = defaultLayerPriority;
                defaultLayerPriority = value;

                if (defaultLayer != null)
                {
                    DoModificationOperation(defaultLayer.SubTypeTypes(), () =>
                    {
                        overlayStack.Objects.Remove(oldPriority);
                        overlayStack.Objects.Add(defaultLayerPriority, defaultLayer);
                    });
                }

            }
        }
        private int defaultLayerPriority = 1000;

        #endregion

        MultiType DefaultLayerOrCreate => defaultLayer ??= new MultiType();

        public MultiType DefaultLayer
        {
            get => defaultLayer;
            set
            {
                if (defaultLayer == value) return;
                if (defaultLayer != null)
                {
                    this.RemoveLayer(DefaultLayerPriority);
                }

                defaultLayer = value;

                if (defaultLayer != null)
                {
                    this.AddLayer(DefaultLayerPriority, defaultLayer);
                }
            }
        }
        private MultiType defaultLayer;

        #endregion

        #region MultiType Pass-through to Stack

        private readonly object overlayStackLock = new object();
        private MultiTypeOverlayStack overlayStack
        {
            get => _overlayStack;
            set => _overlayStack = value;
        }
        private MultiTypeOverlayStack _overlayStack = new MultiTypeOverlayStack();

#if !NoGenericMethods
        public T AsType<T>() where T : class => overlayStack.AsType<T>();
        public IEnumerable<T> OfType<T>() where T : class => overlayStack.OfType<T>();
#endif
        public object AsType(Type T) => overlayStack.AsType(T);
        public IEnumerable<object> OfType(Type T) => overlayStack.OfType(T);

        public IEnumerable<object> SubTypes => overlayStack.SubTypes;

        public bool IsEmpty
        {
            get
            {
                // UNTESTED
                if (false == ((IMultiTyped)defaultLayer).IsEmpty) return false;
                if (overlayStack.Objects.Any()) return false;
                return true;
            }
        }

        public object this[Type type]
        {
            get => overlayStack[type];
            set
            {
#if true // Autocreate DefaultLayer
                if (DefaultLayer == null)
                {
                    DefaultLayer = new MultiType();
                }
                DefaultLayer[type] = value;
#else
                if (DefaultLayer != null)
                {
                    DefaultLayer[type] = value;
                }
                else
                {
                    throw new ArgumentException("Cannot use set methods when a DefaultLayer has not been set");
                }
#endif
            }
        }

        public void SetType(object obj, Type type)
        {
#if true // Autocreate DefaultLayer
            if (DefaultLayer == null)
            {
                DefaultLayer = new MultiType();
            }
            DefaultLayer.SetType(obj, type);
#else
            if (DefaultLayer != null)
            {
                DefaultLayer.SetType(obj, type);
            }
            else
            {
                throw new ArgumentException("Cannot use set methods when a DefaultLayer has not been set");
            }
#endif
        }

        public void ClearSubTypes()
        {
            overlayStack.Objects.Clear();
            OnCleared();
        }

#if !NoGenericMethods
        //T IMultiTyped.AsTypeOrSetDefault<T>(Func<T> defaultValueFunc, Type slotType
        //    //= null
        //    )
        //{
        //    throw new NotImplementedException();
        //}
        //T IMultiTyped.AsTypeOrSetDefault<T>(Func<IMultiTyped, T> defaultValueFunc, Type slotType
        //    //= null
        //    )
        //{
        //    throw new NotImplementedException();
        //}

        //T IMultiTyped.AsTypeOrCreateDefault<T>(Type slotType
        //    //= null
        //    )
        //{
        //    throw new NotImplementedException();
        //}
#endif
        //object IMultiTyped.AsTypeOrCreateDefault(Type slotType, Type type) { throw new NotImplementedException(); }
        #endregion

        #region Overlay Objects

        private readonly object overlayObjectsLock = new object();

#if OverlayProxies
        private Dictionary<Type, IOverlay> overlayObjects;

        public T GetOverlayObject<T>()
            where T : class, new()
        {
            l.Warn("UNTESTED, EXPERIMENTAL: GetOverlayObject");

            lock (overlayObjectsLock)
            {
                if (overlayObjects == null)
                {
                    overlayObjects = new Dictionary<Type, IOverlay>();
                }

                IOverlay overlayObj = overlayObjects.TryGetValue(typeof(T));

                if (overlayObj != null)
                {
                    return (T)overlayObj;
                }

                T overlay = OverlayFactory<T>.Create();
                IOverlay<T> overlayInterface;
                overlayInterface = (IOverlay<T>)overlay;
                overlayInterface.Disposing += OnDisposingOverlayObject;

                if (DefaultLayer != null)
                {
                    T defaultLayerObject = DefaultLayer.AsType<T>();
                    if (defaultLayerObject != null)
                    {
                        overlayInterface.Insert(defaultLayerObject, "__DefaultLayer");
                    }
                }

                overlayObjects.Add(typeof(T), overlayInterface);

                return overlay;
            }
        }
#endif

        void OnDisposingOverlayObject<T>(IOverlay<T> obj)
            where T : class, new()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Collection methods

        /// <summary>
        /// TODO REFACTOR: Use this method for all modification options
        /// Notifies 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="action"></param>
        private void DoModificationOperation(IEnumerable<Type> types, Action action)
        {
            // Notifies

            lock (overlayStackLock)
            {
                var oldValues = new Dictionary<Type, object>();
                foreach (Type type in (IEnumerable)types)
                {
                    oldValues.Add(type, this[type]);
                }

                action();

                foreach (Type type in oldValues.Keys)
                {
                    object newValue = this[type];
                    if (newValue != oldValues[type])
                    {
                        OnChildChanged(type, newValue);
                    }
                }
            }
        }

        //public void AddLayer(int modPriority, object layer)
        //{
        //    DoModificationOperation(new Type[] { layer.GetType() }, () =>
        //    {
        //        MultiType mt = new MultiType(new object[] { layer });
        //        PROBLEM: Need corresponding remove method
        //        overlayStack.Objects.Add(modPriority, mt);
        //    });
        //}

        public void AddLayer(int modPriority, IReadOnlyMultiTyped layer)
        {
            DoModificationOperation(layer.SubTypes.Select(st => st.GetType()), () =>
            {
                overlayStack.Objects.Add(modPriority, layer);
            });

            //lock (overlayStackLock)
            //{
            //    Dictionary<Type, object> oldValues = new Dictionary<Type, object>();
            //    foreach (Type type in layer.SubTypes.Select(st => st.GetType()))
            //    {
            //        oldValues.Add(type, this[type]);
            //    }

            //    overlayStack.Objects.Add(modPriority, layer);

            //    foreach (Type type in oldValues.Keys)
            //    {
            //        object newValue = this[type];
            //        if (newValue != oldValues[type])
            //        {
            //            OnChildChanged(type, newValue);
            //        }
            //    }
            //}
        }

        public bool RemoveLayer(int key)
        {
            lock (overlayStackLock)
            {
                bool removedSomething;
                IReadOnlyMultiTyped layer =
#if AOT
					(IReadOnlyMultiTyped)
#endif
 overlayStack.Objects.TryGetValue(key);

                if (layer == null) return false;


                var oldValues = new Dictionary<Type, object>();
                foreach (object obj in layer.SubTypes)
                //.Select(st => st.GetType())
                {
                    Type type = obj.GetType();
                    oldValues.Add(type, this[type]);
                }

                removedSomething = overlayStack.Objects.Remove(key);

                foreach (Type type in oldValues.Keys)
                {
                    object newValue = this[type];
                    if (newValue != oldValues[type])
                    {
                        OnChildChanged(type, newValue);
                    }
                }

                return removedSomething;
            }
        }

#if !AOT
        public int RemoveLayer(IReadOnlyMultiTyped layer)
        {
            lock (overlayStackLock)
            {
                var oldValues = new Dictionary<Type, object>();
                foreach (Type type in layer.SubTypes.Select(st => st.GetType()))
                {
                    oldValues.Add(type, this[type]);
                }

                int removalCount = 0;
                var matches = overlayStack.Objects.Where(kvp => kvp.Value == layer);
                foreach (var key in matches.Select(x => x.Key))
                {
                    overlayStack.Objects.Remove(key);
                    removalCount++;
                }

                foreach (Type type in oldValues.Keys)
                {
                    object newValue = this[type];
                    if (newValue != oldValues[type])
                    {
                        OnChildChanged(type, newValue);
                    }
                }

                return removalCount;
            }
        }
#endif

        public void ClearLayers()
        {
            lock (overlayStackLock)
            {
                var oldSubTypes = this.SubTypes;

                overlayStack.Objects.Clear();
                OnCleared();

                OnSubTypesRemoved(oldSubTypes);
            }
        }

        protected virtual void OnCleared()
        {
        }

        #endregion

        #region INotifyMultiTypeChanged

        private void OnSubTypesRemoved(IEnumerable<object> oldSubTypes)
        {
#if AOT
			foreach (var x in oldSubTypes)
			{
				var type = x.GetType(); 
				// NOTE: Distinct is missing, may get multiple events per type
#else
            foreach (var type in oldSubTypes.Select(ost => ost.GetType()).Distinct())  // pre-AOT
            {
#endif
                OnChildChanged(type, null);
            }
        }

        #region Implementation

        private readonly Dictionary<Type, Action<IReadOnlyMultiTyped, Type>> handlers = new Dictionary<Type, Action<IReadOnlyMultiTyped, Type>>();
        private readonly object handlersLock = new object();


        public void AddTypeHandler(Type type, Action<IReadOnlyMultiTyped, Type> callback)
        //where T : class
        {
            lock (handlersLock)
            {
                // TODO FIXME REVIEW
                if (!handlers.ContainsKey(type)) handlers.Add(type, callback);
                else handlers[type] += callback;
            }
        }

        public void RemoveTypeHandler(Type type, Action<IReadOnlyMultiTyped, Type> callback)
        //public void RemoveTypeHandler<T>(Type type, MulticastDelegate callback)
        //where T : class
        {
            lock (handlersLock)
            {
                if (!handlers.ContainsKey(type)) return;

                handlers[type] -= callback;
            }
        }

        private void OnChildChanged(Type type, object newValue)
        {
            lock (handlersLock)
            {
                // TODO FIXME REVIEW
                if (!handlers.ContainsKey(type)) return;
                var ev = handlers[type];
                if (ev != null) ev.DynamicInvoke(this, type, newValue);
            }
        }

        public T AsTypeOrCreateDefault<T>(Type slotType = null, Func<T> valueFactory = null) where T : class
        {
            throw new NotImplementedException();
            //return ((IMultiTyped)DefaultLayer).AsTypeOrCreateDefault<T>(slotType);
        }
        public void SetType<T>(T obj) where T : class => DefaultLayerOrCreate.SetType<T>(obj);

        public void AddType<T>(T obj, bool allowMultiple = false) where T : class => DefaultLayerOrCreate.AddType(obj, allowMultiple);

        #endregion

        #endregion

        private static readonly ILogger l = Log.Get();
    }
}
