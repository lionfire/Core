using LionFire.Collections.Concurrent;
using LionFire.ExtensionMethods;
using LionFire.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.MultiTyping;

public interface IMultiTypableVisitor
{
    Type InterfaceType { get; }
    object Value { get; }

    bool AllowMultiple { get; }

    void Enter(IMultiTypable multitypable);
    void Leave();
}
public class MultiTypableVisitor<T, TFactory> : IMultiTypableVisitor
    where T : class
    where TFactory : System.Delegate
{
    public Type InterfaceType => typeof(T);
    public T Value { get; set; }
    object IMultiTypableVisitor.Value => Value;
    public bool AllowMultiple { get; set; }
    TFactory Factory { get; }
    IMultiTypable Multitypable { get; }
    public MultiTypableVisitor(IMultiTypable multitypable, TFactory factory)
    {
        Multitypable = multitypable;
        Factory = factory;
    }

    private IMultiTyped multiTyped { get; set; }
    private bool IsDisposed { get; set; }

    public void Enter(IMultiTypable owner)
    {
        if (Value == null)
        {
            Value = (T)Factory.DynamicInvoke(owner);
        }
        if (Value == null) throw new InvalidOperationException("Factory created null object");
        Multitypable.MultiTyped.AddType<T>(Value, AllowMultiple);
    }
    public void Leave()
    {
        throw new NotImplementedException();
#if TODO
        //multiTyped.Remove<T>(Value); // TODO
        IsDisposed = true;
        multiTyped = null;
#endif
    }
}

public class MultiTypeEqualityComparer : IEqualityComparer<IMultiTyped>
{
    public static IEqualityComparer<IMultiTyped> Default { get; } = new MultiTypeEqualityComparer();

    public bool Equals(IMultiTyped x, IMultiTyped y)
    {
        if (x == null || x.IsEmpty) return y == null || y.IsEmpty;

        throw new NotImplementedException("MultiTyped equality comparison");
        //return true;
    }

    public int GetHashCode([DisallowNull] IMultiTyped obj)
    {
        var hashCode = new HashCode();

        if (obj.SubTypes != null)
            foreach (var child in obj.SubTypes)
            {
                hashCode.Add(child.GetHashCode());
            }
        // REVIEW - is this complete?

        return hashCode.ToHashCode();
    }
}

#if OLD
[Obsolete("Use MultiType instead")]
public class LockingMultiType : IMultiTyped, IMultiTypable // RENAME to MultiTyped
{
    private object _lock = new object();

    #region Construction

    public LockingMultiType() { }
    public LockingMultiType(IEnumerable<object> objects)
    {
        foreach (var obj in objects)
        {
            this.AddType(obj);
        }
    }
    public LockingMultiType(params object[] objects)
    {
        foreach (var obj in objects)
        {
            this.AddType(obj);
        }
    }
    public LockingMultiType(params IMultiTypableVisitor[] items)
    {
        foreach (var item in items)
        {
            this.AddType(item.InterfaceType, item.Value, item.AllowMultiple);
        }
    }

    #endregion

    // ENH: Switch to ConcurrentDictionary? See ConcurrentMultiType below for a start
    protected Dictionary<Type, object>? TypeDict => typeDict;
    protected Dictionary<Type, object>? typeDict;

    public IEnumerable<Type> Types => TypeDict == null ? Enumerable.Empty<Type>() : TypeDict.Keys;

    public IEnumerable<object> SubTypes => TypeDict?.Values ?? Enumerable.Empty<object>();

    [Ignore]
    public IMultiTyped MultiTyped => this;

    public bool IsEmpty
    {
        get
        {
            if (typeDict != null && typeDict.Any()) return false;
            return true;
        }
    }

    public object this[Type type] { get => AsType(type); set => this.SetType(value, type); }

    public T? AsType<T>()
        where T : class
    {
        if (typeDict == null) return null;
        if (!typeDict.ContainsKey(typeof(T))) return null;
        return (T)typeDict[typeof(T)];
    }

    //[AotReplacement]
    public object? AsType(Type type)
    {
        if (typeDict == null) return null;
        if (!typeDict.ContainsKey(type)) return null;
        return typeDict[type];
        //Type slotType = GetSlotType(type);
        //object itemToAdd = this[slotType];
        //return itemToAdd;
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

    public void AddType<T>(T obj, bool allowMultiple = false)
        where T : class
    {
        //if (itemToAdd == default(T)) { UnsetType<T>(); return; }

        lock (_lock)
        {
            if (typeDict == null)
            {
                typeDict = new Dictionary<Type, object>();
            }
            if (typeDict.ContainsKey(typeof(List<T>)))
            {
                if (!allowMultiple) throw new AlreadyException($"Already contains one or more {typeof(T).FullName} and allowMultiple parameter is false.");
                ((List<T>)typeDict[typeof(T)]).Add(obj);
            }
            else if (typeDict.ContainsKey(typeof(T)))
            {
                if (!allowMultiple) throw new AlreadyException($"Already contains a {typeof(T).FullName} and allowMultiple parameter is false.");
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
    public void AddType(Type T, object obj, bool allowMultiple = false)
        => typeof(LockingMultiType).GetMethods()
        .Where(m => m.Name == nameof(AddType) && !m.ContainsGenericParameters).First().MakeGenericMethod(T)
        .Invoke(this, new object[] { obj, allowMultiple });

    public void SetType<T>(T obj)
        where T : class
    {
        if (obj == default(T)) { UnsetType<T>(); return; }

        lock (_lock)
        {
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

    // FUTURE? UnsetType(object itemToAdd) => UnsetType(itemToAdd.GetType()) but only if that value matches the itemToAdd.
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

    T InstantiateOrThrow<T>(Type type)
    {
        var obj = Activator.CreateInstance(type);
        if (obj == null) throw new Exception($"Failed to instantiate: {type.FullName}");
        return (T)obj;
    }

    #region AsTypeOrCreateDefault


#if !NoGenericMethods
    //public T AsTypeOrCreateDefault<T>(Func<T> defaultFactory) where T : class => AsTypeOrCreateDefault<T>(defaultFactory: defaultFactory); // MOVED to extension method

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="slotType">Gets the existing value or set a default value registered for this type, which may be a base class or interface of T.  If not provided, GetSlotType will get the type for T.</param>
    /// <param name="defaultFactory"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public T AsTypeOrCreateDefault<T>(Type? slotType = null, Func<T>? defaultFactory = null)
        where T : class
    {
        slotType ??= GetSlotType(typeof(T));

        object obj = this[slotType];
        if (obj != null) return (T)obj;

        lock (_lock)
        {
            obj = this[slotType];
            if (obj != null) return (T)obj;

            Type concreteType = typeof(T);
            if (concreteType.IsAbstract || concreteType.IsInterface) concreteType = GetDefaultConcreteType(concreteType);

            if (concreteType.IsAbstract || concreteType.IsInterface)
            {
                throw new ArgumentException("Could not determine concrete type for " + typeof(T).FullName + ".  Try adding a DefaultCooncreteTypeAttribute to this non-concrete type.");
            }

            T defaultValue;
            if (defaultFactory != null) defaultValue = defaultFactory();
            else defaultValue = InstantiateOrThrow<T>(concreteType);

            _SetOrReplaceSingle(defaultValue, slotType);
            //SetType<T>(defaultValue);
            return defaultValue;
        }
    }
#endif

#if OLD // AotReplacement no longer used
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
        if (type == null) throw new ArgumentNullException(nameof(type));

        if (slotType == null) { slotType = GetSlotType(type); }

        Type concreteType = type;
        if (concreteType.IsAbstract || concreteType.IsInterface) concreteType = GetDefaultConcreteType(concreteType);

        return AsTypeOrCreateDefault(
            //() => Activator.CreateInstance(type),
            null,
            slotType, type);
    }
#endif

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

    private void _SetOrReplaceSingle(object obj, Type slotType, bool allowReplace = false)
    {
        lock (_lock)
        {
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
                        //if (oldValue == obj) // REVIEW - commented this, don't rely on ==
                        //    return;

                        SetMultiTypeParent(obj);
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
                SetMultiTypeParent(obj);
                typeDict.Add(slotType, obj);
            }
        }
        OnChildChanged(slotType, obj);

        #region (local method)

        void SetMultiTypeParent(object obj)
        {
            if (obj is IHasMultiTypeParent hmt) // REVIEW - needed? can this be avoided?
            {
                if (hmt.MultiTypeParent != null && hmt.MultiTypeParent != this)
                {
                    throw new ArgumentException("IHasMultiTypeParent.MultiTypeParent already set to another parent");
                }
                hmt.MultiTypeParent = this;
            }
        }

        #endregion
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
#endif

// TODO: Make sealed?
// TODO: Supercede with FlexObject? Or is that too wide open?
public class MultiTyped : IMultiTyped, IMultiTypable 
{
    private object _lock = new object();

    #region Construction

    public MultiTyped() { }
    public MultiTyped(IEnumerable<object> objects)
    {
        foreach (var obj in objects)
        {
            this.AddType(obj);
        }
    }
    public MultiTyped(params object[] objects)
    {
        foreach (var obj in objects)
        {
            this.AddType(obj);
        }
    }
    public MultiTyped(params IMultiTypableVisitor[] items)
    {
        foreach (var item in items)
        {
            this.AddType(item.InterfaceType, item.Value, item.AllowMultiple);
        }
    }

    #endregion

    private ConcurrentDictionary<Type, object>? TypeDict => typeDict;
    private ConcurrentDictionary<Type, object>? typeDict;
    private ConcurrentDictionary<Type, object> nonNullTypeDict
    {
        get
        {
            lock (_lock)
            {
                if (typeDict == null)
                {
                    typeDict = new ConcurrentDictionary<Type, object>();
                }
                return typeDict;
            }
        }
    }

    public IEnumerable<Type> Types => TypeDict == null ? Enumerable.Empty<Type>() : TypeDict.Keys;

    public IEnumerable<object> SubTypes => TypeDict?.Values ?? Enumerable.Empty<object>();

    [Ignore]
    IMultiTyped IMultiTypable.MultiTyped => this;

    public bool IsEmpty
    {
        get
        {
            if (typeDict != null && typeDict.Any()) return false;
            return true;
        }
    }

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
        //object itemToAdd = this[slotType];
        //return itemToAdd;
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

    public void AddType<T>(T itemToAdd, bool allowMultiple = false)
        where T : class
    {
        //if (itemToAdd == default(T)) { UnsetType<T>(); return; }

        var d = nonNullTypeDict;

        if (d.TryGetValue(typeof(ConcurrentList<T>), out var listObj))
        {
            var list = (ConcurrentList<T>)listObj;
            Debug.Assert(list.Count > 0);

            if (!allowMultiple) throw new AlreadyException($"Already contains one or more {typeof(T).FullName} and allowMultiple parameter is false.");
            list.Add(itemToAdd);
        }
        else
        {
            lock (_lock)
            { // If AddOrRemove existed, this lock could be eliminated

                if (d.TryGetValue(typeof(T), out var existingObj))
                {
                    var existing = (T)existingObj;
                    if (!allowMultiple) throw new AlreadyException($"Already contains a {typeof(T).FullName} and allowMultiple parameter is false.");

                    d.AddOrUpdate(typeof(ConcurrentList<T>),
                        addValueFactory: key => new ConcurrentList<T>
                        {
                        existing,
                        itemToAdd
                        },
                        updateValueFactory: (key, existingList) =>
                        {
                            ((ConcurrentList<T>)existingList).Add(itemToAdd);
                            return existingList;
                        });

                    var removed = d.TryRemove(typeof(T), out var _);
                    Debug.Assert(removed);

                }
                else
                {
                    if (!d.TryAdd(typeof(T), itemToAdd)) throw new UnreachableCodeException();
                }
            }
        }

    }
    public void AddType(Type T, object obj, bool allowMultiple = false)
        => typeof(MultiTyped).GetMethods()
        .Where(m => m.Name == nameof(AddType) && !m.ContainsGenericParameters).First().MakeGenericMethod(T)
        .Invoke(this, new object[] { obj, allowMultiple });

    public void SetType<T>(T obj) // RENAME SetSingle
        where T : class
    {
        if (obj == default(T)) { UnsetType<T>(); return; }

        if (!nonNullTypeDict.TryAdd(typeof(T), obj))
        {
            throw new ArgumentException($"Already contains an object of type {typeof(T).Name}.  Either remove the previous value or use the Add method to add to a IEnumerable<T> for the type.");
        }
    }

    // Non-generic version of SetType<T>
    public void SetType(object obj, Type type) // RENAME SetSingle
    {
        if (type.IsValueType) throw new NotSupportedException("ValueType not currently supported");
        if (obj == null) { UnsetType(type); return; }

        if (!nonNullTypeDict.TryAdd(type, obj))
        {
            throw new ArgumentException($"Already contains an object of type {type.Name}.  Either remove the previous value or use the Add method to add to a IEnumerable<T> for the type.");
        }
    }

    public bool UnsetType<T>()
    {
        var d = typeDict;
        if (d == null) return false;
        bool foundItem;

        lock (_lock)
        {
            foundItem = d.TryRemove(typeof(T), out var _);
            if (d.Count == 0 && ReferenceEquals(d, typeDict)) typeDict = null;
        }

        return foundItem;
    }

    // FUTURE? UnsetType(object itemToAdd) => UnsetType(itemToAdd.GetType()) but only if that value matches the itemToAdd.
    public bool UnsetType(Type type)
    {
        var d = typeDict;
        if (d == null) return false;
        bool foundItem;

        lock (_lock)
        {
            foundItem = d.TryRemove(type, out var _);
            if (d.Count == 0 && ReferenceEquals(d, typeDict)) typeDict = null;
        }

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

    T InstantiateOrThrow<T>(Type type)
    {
        var obj = Activator.CreateInstance(type);
        if (obj == null) throw new Exception($"Failed to instantiate: {type.FullName}");
        return (T)obj;
    }

    #region AsTypeOrCreateDefault


#if !NoGenericMethods
    //public T AsTypeOrCreateDefault<T>(Func<T> defaultFactory) where T : class => AsTypeOrCreateDefault<T>(defaultFactory: defaultFactory); // MOVED to extension method

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="slotType">Gets the existing value or set a default value registered for this type, which may be a base class or interface of T.</param>
    /// <param name="defaultFactory"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public T AsTypeOrCreateDefault<T>(Type? slotType = null, Func<T>? defaultFactory = null)
        where T : class
    {
        slotType ??= GetSlotType(typeof(T));

        object obj = this[slotType];
        if (obj != null) return (T)obj;

        lock (_lock)
        {
            obj = this[slotType];
            if (obj != null) return (T)obj;

            Type concreteType = typeof(T);
            if (concreteType.IsAbstract || concreteType.IsInterface) concreteType = GetDefaultConcreteType(concreteType);

            if (concreteType.IsAbstract || concreteType.IsInterface)
            {
                throw new ArgumentException("Could not determine concrete type for " + typeof(T).FullName + ".  Try adding a DefaultConcreteTypeAttribute to this non-concrete type.");
            }

            T defaultValue;
            if (defaultFactory != null) defaultValue = defaultFactory();
            else defaultValue = InstantiateOrThrow<T>(concreteType);

            _SetOrReplaceSingle(defaultValue, slotType);
            //SetType<T>(defaultValue);
            return defaultValue;
        }
    }
#endif

#if false // OLD - AOT replacement is no longer used
#endif
    // Type shouldn't be allowed to be null, but this is to allow null slotType, for auto AOT method replacement, since the AOT-Compatlyzer is limited
    [AotReplacement]
    public object AsTypeOrCreateDefault(Func<object> defaultValueFunc = null, Type slotType = null, Type type = null)
    {
        if (type == null) throw new ArgumentNullException("type");
        Type concreteType = type;
        if (concreteType.IsAbstract || concreteType.IsInterface) concreteType = GetDefaultConcreteType(type);

        if (slotType == null) slotType = GetSlotType(type);

        lock (_lock)
        {
            object obj = this[slotType];
            if (obj != null) return obj;

            object defaultValue = defaultValueFunc != null ? defaultValueFunc() : (Activator.CreateInstance(type) ?? throw new Exception($"Failed to create {type.FullName}"));

            //_SetOrReplaceSingle(defaultValue, slotType);
            SetType(defaultValue, slotType);

            return defaultValue;
        }
    }


    [AotReplacement]
    public object AsTypeOrCreateDefault(Type type /* = null */, Type slotType = null)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

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


    private void _SetOrReplaceSingle(object obj, Type slotType, bool allowReplace = false)
    {
        lock (_lock)
        {
            var d = nonNullTypeDict;

            d.AddOrUpdate(slotType,
                addValueFactory: key =>
                {
                    SetMultiTypeParent(obj);
                    return obj;
                },
                updateValueFactory: (key, existingObj) =>
                {
                    if (ReferenceEquals(existingObj, obj)) { return existingObj; }
                    if (!allowReplace)
                    {
                        throw new ArgumentException("Object of specified type is already set but allowReplace is false");
                    }
                    SetMultiTypeParent(obj);
                    return obj; // Replace with new obj
                });
        }
        OnChildChanged(slotType, obj);

        #region (local method)

        void SetMultiTypeParent(object obj)
        {
            if (obj is IHasMultiTypeParent hmt) // REVIEW - needed? can this be avoided?
            {
                if (hmt.MultiTypeParent != null && hmt.MultiTypeParent != this)
                {
                    throw new ArgumentException("IHasMultiTypeParent.MultiTypeParent already set to another parent");
                }
                hmt.MultiTypeParent = this;
            }
        }

        #endregion
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
