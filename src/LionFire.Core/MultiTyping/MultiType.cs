using LionFire.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{
    public class MultiType : IMultiTyped,  IMultiTypable // RENAME to MultiTyped
    {
        private object _lock = new object();

        #region Construction

        public MultiType() { }
        public MultiType(IEnumerable<object> objects)
        {
            foreach (var obj in objects)
            {
                this.AddType(obj);
            }
        }

        #endregion

        // TODO: Switch to ConcurrentDictionary?
        protected Dictionary<Type, object> TypeDict { get { return typeDict; } }
        protected Dictionary<Type, object> typeDict;

        public IEnumerable<Type> Types => TypeDict == null ? Enumerable.Empty<Type>() : TypeDict.Keys;

        public IEnumerable<object> SubTypes => TypeDict?.Values ?? Enumerable.Empty<object>();

        public IMultiTyped MultiTyped => this;

        public object this[Type type] { get => AsType(type); set => this.SetType(value, type); }

        public T AsType<T>()
            where T : class
        {
            if (typeDict == null) return null;
            if (!typeDict.ContainsKey(typeof(T))) return null;
            return (T)typeDict[typeof(T)];
        }

        [AotReplacement]
        public object AsType(Type type)
        {
            if (typeDict == null) return null;
            if (!typeDict.ContainsKey(type)) return null;
            return typeDict[type];
            //Type slotType = GetSlotType(type);
            //object obj = this[slotType];
            //return obj;
        }

        #region OfType

        public IEnumerable<T> OfType<T>() // TODO: Make IEnumerable once LionRpc supports it.
         where T : class
        {
            var matches = new List<T>();
            if (typeDict != null)
            {
                foreach (object obj in typeDict.Values)
                {
                    if (typeof(T).IsAssignableFrom(obj.GetType()))
                    {
                        matches.Add((T)obj);
                    }
                }
            }
            return matches;
        }

        [AotReplacement]
        public IEnumerable<object> OfType(Type T) // TODO: Make IEnumerable once LionRpc supports it.
        {
            var matches = new List<object>();
            if (typeDict != null)
            {
                foreach (object obj in typeDict.Values)
                {
                    if (T.IsAssignableFrom(obj.GetType()))
                    {
                        matches.Add(obj);
                    }
                }
            }
            return matches;
        }

        #endregion

        public void AddType<T>(T obj)
            where T : class
        {
            //if (obj == default(T)) { UnsetType<T>(); return; }

            lock (_lock)
            {
                if (typeDict == null)
                {
                    typeDict = new Dictionary<Type, object>();
                }
                if (typeDict.ContainsKey(typeof(List<T>)))
                {
                    ((List<T>)typeDict[typeof(T)]).Add(obj);
                }
                else if (typeDict.ContainsKey(typeof(T)))
                {
                    var list = new List<T>();
                    list.Add((T)typeDict[typeof(T)]);
                    list.Add(obj);
                    typeDict.Add(typeof(List<T>), list);
                    typeDict.Remove(typeof(T));
                }
                else
                {
                    typeDict.Add(typeof(T), obj);
                }
            }
        }

        public void SetType<T>(T obj)
            where T : class
        {
            if (obj == default(T)) { UnsetType<T>(); return; }

            if (typeDict == null)
            {
                typeDict = new Dictionary<Type, object>();
            }
            if (typeDict.ContainsKey(typeof(T)))
            {
                throw new ArgumentException($"Already contains an object of type {typeof(T).Name}.  Either remove the previous value or use the Add method to add to a IEnumerable<T> for the type.");
            }
            typeDict.Add(typeof(T), obj);
        }
        public void SetType(object obj, Type type)
        {
            if (type.IsValueType) throw new NotSupportedException("ValueType not currently supported");
            if (obj == null) { UnsetType(type); return; }

            if (typeDict == null)
            {
                typeDict = new Dictionary<Type, object>();
            }
            if (typeDict.ContainsKey(type))
            {
                throw new ArgumentException($"Already contains an object of type {type.Name}.  Either remove the previous value or use the Add method to add to a IEnumerable<T> for the type.");
            }
            typeDict.Add(type, obj);
        }

        public bool UnsetType<T>()
        {
            if (typeDict == null) return false;
            bool foundItem = false;
            if (typeDict.ContainsKey(typeof(T)))
            {
                typeDict.Remove(typeof(T));
                foundItem = true;
            }
            if (typeDict.Count == 0) typeDict = null;

            return foundItem;
        }

        public bool UnsetType(Type type)
        {
            if (typeDict == null) return false;
            bool foundItem = false;
            if (typeDict.ContainsKey(type))
            {
                typeDict.Remove(type);
                foundItem = true;
            }
            if (typeDict.Count == 0) typeDict = null;

            return foundItem;
        }


        #region Type Change Events

        private Dictionary<Type, Action<IReadOnlyMultiTyped, Type>> handlers = new Dictionary<Type, Action<IReadOnlyMultiTyped, Type>>();
        private object handlersLock = new object();

        private Dictionary<Type, Action<SReadOnlyMultiTyped, Type>> sHandlers = new Dictionary<Type, Action<SReadOnlyMultiTyped, Type>>();
        private object sHandlersLock = new object();


        public void AddTypeHandler(Type type, Action<IReadOnlyMultiTyped, Type> callback)
        {
            lock (handlersLock)
            {
                // TODO FIXME REVIEW
                if (!handlers.ContainsKey(type)) handlers.Add(type, callback);
                else handlers[type] += callback;
            }
        }

        public void RemoveTypeHandler(Type type, Action<IReadOnlyMultiTyped, Type> callback)
        {
            lock (handlersLock)
            {
                if (!handlers.ContainsKey(type)) return;

                handlers[type] -= callback;
            }
        }

        public void AddTypeHandler(Type type, Action<SReadOnlyMultiTyped, Type> callback)
        {
            lock (sHandlersLock)
            {
                // TODO FIXME REVIEW
                if (!sHandlers.ContainsKey(type)) sHandlers.Add(type, callback);
                else sHandlers[type] += callback;
            }
        }

        public void RemoveTypeHandler(Type type, Action<SReadOnlyMultiTyped, Type> callback)
        //public void RemoveTypeHandler<T>(Type type, MulticastDelegate callback)
        //where T : class
        {
            lock (sHandlersLock)
            {
                if (!sHandlers.ContainsKey(type)) return;

                sHandlers[type] -= callback;
            }
        }

        private void OnChildChanged(Type type, object newValue)
        {

            if (newValue is IHasMultiTypeParent hmt)
            {
                if (hmt.MultiTypeParent != null && hmt.MultiTypeParent != this)
                {
                    // l.TraceWarn("IHasMultiTypeParent.MultiTypeParent of type " + hmt.MultiTypeParent.GetType().Name + " was already set to another parent for child of type " + newValue.GetType().Name + ". " + Environment.StackTrace); // TODO WARN Developer
                }
                hmt.MultiTypeParent = this;
            }

            lock (handlersLock)
            {
                // TODO FIXME REVIEW
                if (handlers.ContainsKey(type))
                {
                    var ev = handlers[type];
                    if (ev != null) ev.DynamicInvoke(this, type);
                    //if (ev != null) ev.DynamicInvoke(this, type, newValue);
                }
            }
            lock (sHandlersLock)
            {
                // TODO FIXME REVIEW
                if (sHandlers.ContainsKey(type))
                {
                    var ev = sHandlers[type];
                    if (ev != null) ev.DynamicInvoke(this, type);
                    //if (ev != null) ev.DynamicInvoke(this, type, newValue);
                }
            }
        }

        #endregion

        #region Clear

        public void ClearSubTypes()
        {
            ClearSubTypes(true);
        }
        public void ClearSubTypes(bool disposeSubTypes = true, bool fireEvents = true)
        {
            if (typeDict == null) return;
            foreach (var kvp in typeDict.ToArray())
            {
                if (fireEvents) { OnChildChanged(kvp.Key, null); }
                if (disposeSubTypes)
                {
                    (kvp.Value as IDisposable)?.Dispose();
                }
            }
            typeDict.Clear();
            typeDict = null;
        }

        #endregion

        #region AsTypeOrCreateDefault


#if !NoGenericMethods
        public T AsTypeOrCreateDefault<T>(Type slotType = null)
            where T : class
            //, new()
        {
            if (slotType == null) { slotType = GetSlotType(typeof(T)); }

            object obj = this[slotType];
            if (obj != null) return (T)obj;

            Type concreteType = typeof(T);
            if (concreteType.IsAbstract || concreteType.IsInterface) concreteType = GetDefaultConcreteType(concreteType);

            if (concreteType.IsAbstract || concreteType.IsInterface)
            {
                throw new ArgumentException("Could not determine concrete type for " + typeof(T).FullName + ".  Try adding a DefaultCooncreteTypeAttribute to this non-concrete type.");
            }

            T defaultValue = (T)Activator.CreateInstance(concreteType);


            _Set(defaultValue, slotType);
            //SetType<T>(defaultValue);
            return defaultValue;
        }
#endif

        // Type shouldn't be allowed to be null, but this is to allow null slotType, for auto AOT method replacement, since the AOT-Compatlyzer is limited
        [AotReplacement]
        public object AsTypeOrCreateDefault(Func<object> defaultValueFunc = null, Type slotType = null, Type type = null)
        {
            if (type == null) throw new ArgumentNullException("type");
            Type concreteType = type;
            if (concreteType.IsAbstract || concreteType.IsInterface) concreteType = GetDefaultConcreteType(type);

            if (slotType == null) slotType = GetSlotType(type);

            object obj = this[slotType];
            if (obj != null) return obj;

            object defaultValue;

            if (defaultValueFunc != null)
            {
                defaultValue = defaultValueFunc();
            }
            else
            {
                defaultValue = Activator.CreateInstance(type);
            }

            _Set(defaultValue, slotType);
            //SetType(defaultValue, slotType);
            return defaultValue;
        }

        [AotReplacement]
        public object AsTypeOrCreateDefault(Type type /* = null */, Type slotType = null)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (slotType == null) { slotType = GetSlotType(type); }

            Type concreteType = type;
            if (concreteType.IsAbstract || concreteType.IsInterface) concreteType = GetDefaultConcreteType(concreteType);

            return AsTypeOrCreateDefault(
                //() => Activator.CreateInstance(type),
                null,
                slotType, type);
        }

        #endregion

        #region Slot / Concrete Type Resolution

        // REVIEW From Legacy

        private static Type GetSlotType(Type type)
        {

            var attr = type.GetCustomAttributes(typeof(MultiTypeSlotAttribute), false).OfType<MultiTypeSlotAttribute>().FirstOrDefault();
            if (attr != null)
            {

#if CalleeSanityChecks
                // Check that this type is a base type. (Or do this in the attribute?)
                if (!attr.Type.IsAssignableFrom(type)) { throw new ArgumentException("!{MultiTypeSlotAttribute}.Type.IsAssignableFrom(type) on type " + type.FullName); }
#endif
                type = attr.Type;
            }
            return type;
        }

        private static Type GetDefaultConcreteType(Type type)
        {
            if (!type.IsAbstract && !type.IsInterface) { return type; }
            var attr = type.GetCustomAttribute<DefaultConcreteTypeAttribute>();
#if CalleeSanityChecks
            if (type == attr.Type) throw new ArgumentException("DefaultConcreteTypeAttribute.Type refers to the same type the attribute was applied to");
#endif

            type = attr.Type;
            if (type.IsAbstract || type.IsInterface)
            {
                //if (type.IsAbstract || type.IsInterface)
                //{
                //    throw new ArgumentException("Could not determine concrete type for: " + type.FullName);
                //}
                return GetDefaultConcreteType(type);
            }
            else
            {
                return type;
            }
        }

        #endregion

        #region REVIEW - Imported from Legacy

        private void _Set(object obj, Type slotType, bool allowReplace = false)
        {
            if (obj is IHasMultiTypeParent hmt && hmt.MultiTypeParent != null && hmt.MultiTypeParent != this) { throw new ArgumentException("IHasMultiTypeParent.MultiTypeParent already set to another parent"); }

            if (typeDict == null) typeDict = new Dictionary<Type, object>();

            if (typeDict.ContainsKey(slotType))
            {
                if (!object.ReferenceEquals(typeDict[slotType], obj))
                {
                    if (!allowReplace)
                    {
                        throw new ArgumentException("Object of specified type is already set but allowReplace is false");
                    }
                    else
                    {
                        var oldValue = typeDict[slotType];
                        if (oldValue == obj)
                            return;

                        typeDict[slotType] = obj;
                    }
                }
                else
                {
                    return; // No event
                }
            }
            else
            {
                typeDict.Add(slotType, obj);
            }

            OnChildChanged(slotType, obj);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Warning: Object can be reused after disposing.
        /// REVIEW - From Legacy  - Should this be disposable?
        /// </summary>
        public virtual void Dispose()
        {
            ClearSubTypes(true, false);
        }

        #endregion
    }

}
