#define SanityChecks
//#define TRACE_RETRIEVE
//#define TRACE_VALUECHANGED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using LionFire.Types;
using System.Collections.Specialized;
using LionFire.Collections;
using System.Collections;
//using LionFire.Extensions.Rpc;
//using LionFire.Reflection;
using System.Windows;
using System.Threading;
//using System.Windows.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using LionFire.MultiTyping;
using LionFire.Structures;

namespace LionFire.Bindings
{

    public class LionBindingNode : INotifyCollectionChanged, IDisposable
    {

        private static List<LionBindingNode> Bindings = new List<LionBindingNode>();
        private static object bindingsLock = new object();

        #region Configuration

        public static readonly bool NotifyOnLocalSet = true;

        public const bool AllowAsyncGet = true; // false BLOCKS / SLOW without this on PropertyChanged events?  But those events should be one way so it shouldn't cause a deadlock
        public const bool AllowAsyncSet = true;

        // True will return a possibly invalid default result, but false will block
        public const bool AllowAsyncGetOnFirstRetrieve =
            true;
        //false;

        #endregion

        #region Parameters

        #region IMultiType support

        public bool IsMultiTypeAccessor {
            get { return isMultiTypeAccessor; }
        }
        private readonly bool isMultiTypeAccessor;

        public string MultiTypeTypeName {
            get { if (!IsMultiTypeAccessor) return null; return PropertyName.Substring(1, PropertyName.Length - 2); }
        }

        public Type MultiTypeType {
            get {
                string typeName = MultiTypeTypeName;
                if (typeName == null) return null;

                Type type = TypeResolver.Resolve(typeName);

                //Type type = Type.GetType(typeName);
                //l.Trace("UNTESTED - MultiTypeType: " + (type == null ? "null" : type.FullName));
                return type;
            }
        }

        #endregion

        private Type PropertyType;

        protected MethodInfo GetMethodInfo;
        protected MethodInfo SetMethodInfo;
        public PropertyInfo PropertyInfo {
            get {
                if (__propertyInfo == null)
                {
                    UpdatePropertyInfo();
                }
                return __propertyInfo;
            }
            private set {
                if (__propertyInfo == value) return;

                __propertyInfo = value;

                if (__propertyInfo != null)
                {
                    GetMethodInfo = __propertyInfo.GetGetMethod();
                    SetMethodInfo = __propertyInfo.GetSetMethod();
                }
                else
                {
                    GetMethodInfo = null;
                    SetMethodInfo = null;
                }
                UpdatePropertyCollectionTypes();
            }
        }
        private PropertyInfo __propertyInfo;

        private void UpdatePropertyInfo()
        {
            // FUTURE: Support for specifying the actual propertyinfo in an Expression Tree

            if (BindingObject != null) PropertyInfo = null;

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            if (PropertyName.StartsWith("(") && PropertyName.EndsWith(")"))
            {
                l.LogCritical("UNTESTED: Explicit interface property");
                string propertyNameInt = PropertyName.TrimStart('(').TrimEnd(')');
                int lastIndex = propertyNameInt.LastIndexOf('.');
                string interfaceName = propertyNameInt.Substring(0, lastIndex);
                string interfacePropertyName = propertyNameInt.Substring(lastIndex + 1, propertyNameInt.Length - lastIndex + 1);

                Type interfaceType = Type.GetType(interfaceName);
                PropertyInfo = interfaceType.GetProperty(PropertyName, bindingFlags);
                return;
            }
            else
            {
                Type objectType = BindingObject.GetType();
                var pi = objectType.GetProperty(PropertyName, bindingFlags);

                if (pi != null)
                {
                    PropertyInfo = pi;
                    return;
                }

                foreach (Type type in objectType.GetInterfaces())
                {
                    pi = type.GetProperty(PropertyName, bindingFlags);

                    if (pi != null)
                    {
                        PropertyInfo = pi;
                        return;
                    }
                }

                if (__propertyInfo == null) throw new LionBindingException("Could not find property '" + propertyName + "' in type " + BindingObject.GetType().FullName);
            }
        }

        public bool CanWrite {
            get {

                var pi = PropertyInfo;
                if (pi == null)
                {
                    if (IsMultiTypeAccessor)
                    {
                        return SetMethod != null;
                    }
                    else
                    {
                        l.Info("UNEXPECTED code path: CanWrite");
                    }
                }
                return pi.CanWrite;
            }
        }

        public bool CanRead {
            get {
                var pi = PropertyInfo;
                if (pi == null)
                {
                    if (IsMultiTypeAccessor) // TODO CLEANUP this and CanWrite.  For Properties and everyone, maybe just standardize on PropertyGetMethod != null.
                    {
                        return GetMethod != null;
                    }
                    else
                    {
                        l.Info("UNEXPECTED code path: CanRead");
                    }
                }
                return pi.CanRead;
            }
        }

        #endregion

        #region Future Parameters

        // FUTURE: Dynamic binding using property name instead of property info

        private string PropertyName {
            get { return propertyName; }
        }
        private readonly string propertyName;

        #region ExpectedType

        public Type ExpectedType {
            get;
            set;
            //{
            //    return BindingObject.GetType();
            //        //expectedType ?? 
            //        //propertyInfo.DeclaringType;
            //}
            //set { expectedType  = value; } // Disabled for now.  FUTURE Dynamic support - consider allowing this when PropertyName instead of PropertyInfo is supported
        }
        //private Type expectedType ;

        #endregion

        #endregion

        #region State

        public bool IsBound { get { return isBound; } }
        private bool isBound;

        private bool isBoundToNotifyPropertyChanged;
        private bool isBoundToPropertyChanged;
        private bool isBoundToChangedMethod;
        private bool isBoundToBoolChangedMethod;
        private bool isBoundToStringChangedMethod;
        private bool isBoundToFloatChangedMethod;
        private bool isBoundToIntChangedMethod;
        //private bool isBoundToChangedNewValueMethod;
        //private bool isBoundToChangedOldNewValueMethod;

        private bool isBoundToMultiTypeMethod;
        private bool isBoundToSMultiTypeMethod;

        internal LionBindingNode NextBinding {
            get;
            set;
        }

        #endregion

        private bool isTargetNode;

        #region Construction

        #region LionBinding

        public LionBinding LionBinding {
            get { return lionBinding; }
            set {
                if (lionBinding == value) return;
                if (lionBinding != default(LionBinding)) throw new NotSupportedException("LionBinding can only be set once.");
                lionBinding = value;
            }
        }
        private LionBinding lionBinding;

        #endregion

        public LionBindingNode(LionBinding lionBinding, string propertyName, bool isTargetNode)
        {
            this.isTargetNode = isTargetNode;
            this.lionBinding = lionBinding;
            this.propertyName = propertyName;
            isMultiTypeAccessor = PropertyName.StartsWith("<") && PropertyName.EndsWith(">");

            SetDefaultAccessorMethods();
            UpdateAccessorWrapperMethods();
        }

        private void SetDefaultAccessorMethods()
        {
            if (isMultiTypeAccessor)
            {
                Type type = MultiTypeType;
                //if (type == null) throw new Exception("Could not load type from parameter: " + propertyName);
                if (type == null)
                {
                    l.Error("Could not load type from parameter: " + propertyName + ". You may need to add the type to a ITypeResolver.");
                    //throw new Exception("Could not load type from parameter: " + propertyName);
                    return;
                }
                else
                {
                    MethodInfo miGet = typeof(SReadOnlyMultiTypedEx).GetMethod("AsType", new Type[] { }).MakeGenericMethod(type);
                    GetMethod = (o) => CachedValue = miGet.Invoke(o, null);

                    ///////////////////// ---------  --------- ------------------  

                    //MethodInfo miSet = typeof(IExtendableMultiTyped).GetMethod("SetType").MakeGenericMethod(type);
                    if (typeof(IMultiTyped).IsAssignableFrom(type))
                    {
                        MethodInfo miSet = typeof(IMultiTyped).GetMethod("SetType");
                        SetMethod = (o, val) =>
                            {
                                try
                                {
                                    miSet.Invoke(o, new object[] { val, type });
                                }
                                catch (Exception ex)
                                {
                                    l.Error(this.ToString() + ": SetMethodWrapper threw exception: " + ex);
                                }
                            };
                    }
                    else
                    {
                        SetMethod = null;
                    }
                }
            }
            else
            {
                GetMethod = PropertyGetMethodDefault;
                SetMethod = SetMethodDefault;
            }
        }

        public static Func<object, (bool succeeded, Func<LionBindingNode, object, object> GetMethodWrapper, Action<LionBindingNode, object, object> SetMethodWrapper)> TryUpdateAccessorWrapperMethods { get; set; } = null;


        private void UpdateAccessorWrapperMethods()
        {
            throw new NotImplementedException("TODO: fix depObj");
            //if (TryUpdateAccessorWrapperMethods != null)
            //{
            //    var result = TryUpdateAccessorWrapperMethods(depObj);
            //    if (result.succeeded)
            //    {
            //        GetMethodWrapper = (p1) => result.GetMethodWrapper(this,p1);
            //        SetMethodWrapper = (p1, p2) => result.SetMethodWrapper(this, p1, p2);
            //        return;
            //    }
            //}

            //if (isAsync)
            //{
            //    GetMethodWrapper = AllowAsyncGet ? GetWrapperForAsync : new Func<object, object>((o) => GetMethod(o));
            //    SetMethodWrapper = SetWrapperForAsync;
            //    return;
            //}

            //GetMethodWrapper = (o) => GetMethod(o);
            //SetMethodWrapper = (o, value) => SetMethod(o, value);
            //return;

        }

        private object GetWrapperForAsync(object o)
        {
            if (!AllowAsyncGetOnFirstRetrieve && !hasRetrieved)
            {
                l.Warn("R " + this.ToString() + " BLOCKING a normally async get call");
                return GetMethod(o);
            }

            //Task task = 
            Task.Factory.StartNew(() =>
            {
                int asyncRetries = this.LionBinding.AsyncRetries;
                Exception exception;
                do
                {
                    try
                    {
                        //CachedValue = 
                        GetMethod(o);
                        return;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        l.Warn("Binding: Async get exception: " + ex.ToString());
                        var ev = this.ExceptionThrown; if (ev != null) ev(this, ex);
                    }
                } while (asyncRetries-- > 0);

                l.Error("Binding: Async get exception failed " + this.LionBinding.AsyncRetries + " times.  Final exception: " + exception.ToString());
            });

            if (CachedValue == null && this.PropertyType != null)
            {
                _cachedValue = GetDefault(PropertyType);
            }

            return CachedValue; // For now, return last known value
        }

        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        private void SetWrapperForAsync(object o, object value)
        {
            Task.Factory.StartNew(() =>
            {
                int asyncRetries = this.LionBinding.AsyncRetries;
                Exception exception;
                do
                {

                    try
                    {
                        SetMethod(o, value);
                        return;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        l.Warn("Binding: Async set exception: " + ex.ToString());
                        var ev = this.ExceptionThrown; if (ev != null) ev(this, ex);
                    }
                } while (asyncRetries-- > 0);

                l.Error("Binding: Async get exception failed " + this.LionBinding.AsyncRetries + " times.  Final exception: " + exception.ToString());
            });

        }

        private object PropertyGetMethodDefault(object o)
        {
            try
            {

                return CachedValue =
#if AOT
                    // TOOPTIMIZE - FastInvoke
                    GetMethodInfo.Invoke(o, null);
#else
                    PropertyInfo.GetValue(o, null);
#endif
            }
            catch (Exception ex)
            {
                l.Error(ex.ToString());
                throw;
            }
        }

        private void SetMethodDefault(object obj, object val)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            //try
            //{
            if (PropertyInfo.CanWrite)
            {
                if (NextBinding == null)
                {
                    var conversionMethod = isTargetNode ? this.LionBinding.ToTargetConversion : this.LionBinding.ToSourceConversion;
                    if (conversionMethod != null)
                    {
                        val = conversionMethod(val);
                    }
                    //#error TODO: Always get INotifyingColl as array, (but attach to its change event)
#if AOT
                    // TOOPTIMIZE - FastInvoke
                    try
                    {
                        if (SetMethodInfo != null)
                        {
                            SetMethodInfo.Invoke(obj, new object[] { val });
                        }
                        else
                        {
                            l.Warn("SetMethodInfo was not set");
                            SetMethodInfo = PropertyInfo.GetSetMethod();
                            if (SetMethodInfo != null)
                            {
                                SetMethodInfo.Invoke(obj, new object[] { val });
                            }
                            else  // TEMP
                            {
                                if (PropertyInfo.GetSetMethod() == null)
                                {
                                    l.Warn(PropertyInfo.DeclaringType + "." + PropertyInfo.Name + " PropertyInfo.GetSetMethod() == null");
                                }
                                if (PropertyInfo.GetSetMethod(true) == null)
                                {
                                    l.Warn("PropertyInfo.GetSetMethod(true) == null");
                                }
                                l.Warn("SetMethodInfo could not be retrieved from PropertyInfo!?  Falling back to PropertyInfo");

                                PropertyInfo.SetValue(obj, val, null);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        l.Error("SetMethod threw exception on " + obj.ToStringSafe() + ": " + ex.ToString());
                        throw;
                    }
#else
                    PropertyInfo.SetValue(obj, val, null);
#endif
                }
            }
            else
            {
                l.Debug("Can't set because property.CanWrite is false.");
            }
            //    DependencyObject depObj = o as DependencyObject;
            //    if (depObj != null && !depObj.Dispatcher.CheckAccess())
            //    {
            //        depObj.Dispatcher.BeginInvoke(new Action(() =>
            //            PropertyInfo.SetValue(o, val, null)
            //            ));
            //    }
            //    else
            //    {
            //        //IProxyTargetAccessor proxy = obj as IProxyTargetAccessor;
            //        //if (proxy != null)
            //        //{
            //        //    using (new LionRpcSettings() { IsOneWay = true })
            //        //    {
            //        //        PropertyInfo.SetValue(o, val, null);
            //        //    }
            //        //}
            //        //else
            //        {
            //            PropertyInfo.SetValue(o, val, null);
            //        }

            //    }
            //}
            //}
            //catch (Exception ex)
            //{
            //    l.Error(ex.ToString());
            //    throw;
            //}
        }

        //public LionBindingNode(PropertyInfo propertyInfo)
        //{
        //    this.propertyInfo = propertyInfo;
        //}

        //private static List<Type> GetCollectionTypes(Type type) // MOVE UTILITY - Duplicate
        //{
        //    List<Type> types = new List<Type>();
        //    foreach (Type interfaceType in type.GetInterfaces())
        //    {
        //        if (!interfaceType.IsGenericType) continue;
        //        Type genericType = interfaceType.GetGenericTypeDefinition();
        //        if (genericType == typeof(ICollection<>))
        //        {
        //            //types.Add(interfaceType.GetGenericArguments()[0]);
        //            types.Add(interfaceType);
        //        }
        //    }
        //    return types;
        //}

        #endregion

        #region BindingObject

        protected Type BindingObjectType {
            get { return bindingObjectType; }
            set {
                if (bindingObjectType != null)
                {
                    propertyChangedEventInfo = null;
                }
                bindingObjectType = value;
            }
        }
        private Type bindingObjectType;

        public object BindingObject {
            get { return bindingObject; }
            set {
                if (bindingObject == value) return;

                if (value != null && (ExpectedType != null && !ExpectedType.IsAssignableFrom(value.GetType())))
                {
                    throw new LionBindingException("Specified BindingObject of type '" + value.GetType().FullName + "' does not match expected type for this Subbinding: " + ExpectedType.FullName);
                }

                var oldValue = bindingObject;

                if (bindingObject != null)
                {
                    Detach();
                }

                bindingObject = value;
                _cachedValue = null;
                CachedValueType = null;
                UpdateIsAsync();

                if (isMultiTypeAccessor)
                {
                    this.PropertyType = this.MultiTypeType;
                }
                else
                {
                    var pi = this.PropertyInfo;
                    if (pi != null)
                    {
                        this.PropertyType = pi.PropertyType;  // To get default value in an async get
                    }
                    else
                    {
                        this.PropertyType = null;
                    }
                }

                if (bindingObject != null)
                {
                    BindingObjectType = bindingObject.GetType();
                    Attach();

                }
                else
                {
                    BindingObjectType = null;
                }

                UpdateAccessorWrapperMethods();

                var ev = BindingObjectChanged;
                if (ev != null) ev(oldValue, bindingObject);
            }
        }
        private object bindingObject;

        public event ValueChangedHandler<object> BindingObjectChanged;

        #region IsAsync

        /// <summary>
        /// Sets IsAsync to true if the BindingObject is a proxy, or false otherwise.
        /// </summary>
        private void UpdateIsAsync()
        {
            bool newIsAsync = false;

            if (this.LionBinding.AutoAsync)
            {
                if (bindingObject == null) return;
                if (bindingObject.GetType().Name.EndsWith("Proxy")) // HARDCODE: FUTURE: Extensible Proxy detection
                {
                    newIsAsync = true;
                }
#if SanityChecks
                else if (bindingObject.GetType().Name.Contains("Proxy"))
                {
                    l.Warn("Is this a proxy type:? " + bindingObject.GetType().Name);
                }
#endif
            }
            IsAsync = newIsAsync;
        }

        #region IsAsync

        public bool IsAsync {
            get { return isAsync; }
            set {
                isAsync = value;
            }
        }
        private bool isAsync;

        #endregion


        #endregion

        #endregion

        //private EventInfo PropertyChangedToEventInfo// REFACTOR to be more precise when probing?
        private EventInfo PropertyChangedEventInfo {
            get {
                if (propertyChangedEventInfo == null)
                {
                    if (bindingObject != null)
                    {
                        #region FUTURE

                        //propertyChangedEventInfo = bindingObject.GetType().GetEvent(propertyName + "ChangedForFromTo");
                        //if (propertyChangedEventInfo != null) return propertyChangedEventInfo;

                        //propertyChangedEventInfo = bindingObject.GetType().GetEvent(propertyName + "ChangedFromTo");
                        //if (propertyChangedEventInfo != null) return propertyChangedEventInfo;

                        #endregion

                        propertyChangedEventInfo = bindingObject.GetType().GetEvent(propertyName + "ChangedTo");
                        if (propertyChangedEventInfo != null) return propertyChangedEventInfo;

                        propertyChangedEventInfo = bindingObject.GetType().GetEvent(propertyName + "Changed");
                        if (propertyChangedEventInfo != null) return propertyChangedEventInfo;
                    }
                }
                return propertyChangedEventInfo;
            }
        }
        private EventInfo propertyChangedEventInfo;

        private void MultiTypeObjectChanged(IReadOnlyMultiTyped sender, Type type)
        {
            if (MultiTypeType == type)
            {
                l.Trace("UNTESTED - Got MultiTypeObjectChanged for " + type.FullName);

                try
                {
                    GetValue();

                }
                catch (Exception ex)
                {
                    l.Error("MultiTypeObjectChanged - GetValue failed: " + ex);
                }
            }
            else
            {
                l.Warn("UNTESTED - Ignoring MultiTypeObjectChanged for " + type.FullName + ", " + MultiTypeType.FullName);
            }
        }

        private void SMultiTypeObjectChanged(SReadOnlyMultiTypedEx sender, Type type)
        {
            if (MultiTypeType == type)
            {
                l.Trace("UNTESTED - Got SMultiTypeObjectChanged for " + type.FullName);

                GetValue();
            }
            else
            {
                l.Warn("UNTESTED - Ignoring SMultiTypeObjectChanged for " + type.FullName + ", " + MultiTypeType.FullName);
            }
        }


        private object GetValue()
        {
            RetrieveValue();
            object val = Value;
            return val;
        }

        private void Attach()
        {
            if (IsAsync)
            {
                Task.Factory.StartNew(() =>
                    _Attach()
                    );
            }
            else
            {
                _Attach();
            }
        }
        private void _Attach()
        {
            if (bindingObject == null) throw new ArgumentNullException("bindingObject must not be null");

            isBound = false;

            if (IsMultiTypeAccessor)
            {
                var multiTyped = bindingObject as IReadOnlyMultiTyped;
                var extendableMultiTyped = bindingObject as IMultiTyped;
                INotifyMultiTypeChanged notifyMultiTypeChanged = bindingObject as INotifyMultiTypeChanged;
                SNotifyMultiTypeChanged sNotifyMultiTypeChanged = bindingObject as SNotifyMultiTypeChanged;

                if (notifyMultiTypeChanged != null)
                {
                    notifyMultiTypeChanged.AddTypeHandler(MultiTypeType, MultiTypeObjectChanged);
                    isBoundToMultiTypeMethod = true;
                    isBound = true;
                }
                else if (sNotifyMultiTypeChanged != null)
                {
                    sNotifyMultiTypeChanged.AddTypeHandler(MultiTypeType, SMultiTypeObjectChanged);
                    isBoundToSMultiTypeMethod = true;
                    isBound = true;
                }
                else
                {
                    if (multiTyped == null && bindingObject != null)
                    {
                        l.Error("UNEXPECTED: BindingObject is not of type IReadOnlyMultiTyped: " + bindingObject.GetType().FullName);
                    }
                    if (bindingObject != null && !isBound)
                    {
                        l.Warn("Did not bind to MultiType object due to lack of event support: " + bindingObject.GetType().Name);
                    }
                }
            }
            else
            #region PropertyChanged methods
            {
                //eventInfo = bindingObject.GetType().GetEvent(propertyName + "Changed");
                if (PropertyChangedEventInfo != null)
                {
                    if (PropertyChangedEventInfo.EventHandlerType == typeof(Action))
                    {
#if !AOT
                        PropertyChangedEventInfo.AddEventHandler(bindingObject, new Action(OnPropertyChangedMethod));
#else // TODO: Detach
                        PropertyChangedEventInfo.GetAddMethod().Invoke(bindingObject, new object[] { new Action(OnPropertyChangedMethod) });
#endif
                        isBoundToChangedMethod = true;
                        isBound = true;
                        goto doneBinding;
                    }

                    if (PropertyChangedEventInfo.EventHandlerType == typeof(Action<bool>))
                    {
#if !AOT
                        PropertyChangedEventInfo.AddEventHandler(bindingObject, new Action<bool>(OnBoolPropertyChangedMethod));
#else // TODO: Detach
                        PropertyChangedEventInfo.GetAddMethod().Invoke(bindingObject, new object[] { new Action<bool>(OnBoolPropertyChangedMethod) });
#endif
                        isBoundToBoolChangedMethod = true;
                        isBound = true;
                        goto doneBinding;
                    }

                    if (PropertyChangedEventInfo.EventHandlerType == typeof(Action<string>))
                    {
#if !AOT
                        PropertyChangedEventInfo.AddEventHandler(bindingObject, new Action<string>(OnStringPropertyChangedMethod));
#else // TODO: Detach
                        PropertyChangedEventInfo.GetAddMethod().Invoke(bindingObject, new object[] { new Action<string>(OnStringPropertyChangedMethod) });
#endif
                        isBoundToStringChangedMethod = true;
                        isBound = true;
                        goto doneBinding;
                    }

                    if (PropertyChangedEventInfo.EventHandlerType == typeof(Action<float>))
                    {
#if !AOT
                        PropertyChangedEventInfo.AddEventHandler(bindingObject, new Action<float>(OnFloatPropertyChangedMethod));
#else // TODO: Detach
                        PropertyChangedEventInfo.GetAddMethod().Invoke(bindingObject, new object[] { new Action<float>(OnFloatPropertyChangedMethod) });
#endif
                        isBoundToFloatChangedMethod = true;
                        isBound = true;
                        goto doneBinding;
                    }

                    if (PropertyChangedEventInfo.EventHandlerType == typeof(Action<int>))
                    {
#if !AOT
                        PropertyChangedEventInfo.AddEventHandler(bindingObject, new Action<int>(OnIntPropertyChangedMethod));
#else // TODO: Detach
                        PropertyChangedEventInfo.GetAddMethod().Invoke(bindingObject, new object[] { new Action<int>(OnIntPropertyChangedMethod) });
#endif
                        isBoundToIntChangedMethod = true;
                        isBound = true;
                        goto doneBinding;
                    }

                    //Type newValMethodType = typeof(Action<>).MakeGenericType(this.PropertyInfo.PropertyType);
                    //if (PropertyChangedEventInfo.EventHandlerType == newValMethodType)
                    //{
                    //    PropertyChangedEventInfo.AddEventHandler(bindingObject, Activator.CreateInstance(newValMethodType, ));
                    //    isBoundToChangedNewValueMethod = true;
                    //    goto doneBinding;
                    //}

                    //if(eventInfo.EventHandlerType == typeof(PropertyChangedHandler<>))
                    //{
                    //    isBoundToChangedOldNewValueMethod = true;
                    //    goto doneBinding;
                    //}
                }
            }
            #endregion

            #region IPropertyChanged
            {
                if (bindingObject is IPropertyChanged propertyChanged)
                {
                    propertyChanged.PropertyValueChanged += new Action<string>(OnPropertyChanged);
                    isBoundToPropertyChanged = true;
                    isBound = true;
                    goto doneBinding;
                }
            }
            #endregion

            #region INotifyPropertyChanged
            {
                INotifyPropertyChanged notifyPropertyChanged = bindingObject as INotifyPropertyChanged;
                if (notifyPropertyChanged != null)
                {
                    notifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(notifyPropertyChanged_PropertyChanged);
                    isBoundToNotifyPropertyChanged = true;
                    isBound = true;
                    goto doneBinding;
                }
            }
        #endregion


        // Not required!  Either this should be a non-notifying node, or ___Changed events should exist.  FUTURE - ctor option to indicate mandatory?
        //throw new NotSupportedException("BindingObject must support INotifyPropertyChanged or IPropertyChanged.");

        doneBinding:

            IsBoundToNotifyCollectionChanged = true; // Attempts to be bound to collection.  Reverts to false on fail.

            if (!IsMultiTypeAccessor && !PropertyInfo.CanRead)
            {
                l.Trace("UNTESTED - !PropertyInfo.CanRead support");
            }
            else
            {
                if (NextBinding != null || (isTargetNode ? LionBinding.ToSource : LionBinding.ToTarget))
                {
                    TryEnsureRetrieved();
                    //RetrieveValue(); // OLD - caused recursive deadlock?
                }
                //CachedValue =  this.Value;
            }

            ILionBindingHost lionBindingHost = bindingObject as ILionBindingHost;
            if (lionBindingHost != null)
            {
                lionBindingHost.Bindings.Add(this);
            }
            else
            {
                lock (bindingsLock)
                {
                    Bindings.Add(this);
                }
            }
        }

        //public const bool EnforceIsRetrieveEnabled = true; // Setting to true causes problems: Teams need a manual refresh
        public static readonly bool EnforceIsRetrieveEnabled = false;
        private bool IsRetrieveEnabled {
            get { return NextBinding != null || (isTargetNode ? LionBinding.ToSource : LionBinding.ToTarget) || IsValueCollection; }
        }

        // Future design: a chain of methods may be more appropriate tha a GetMethod and GetMethodWrapper duo.
        protected Func<object, object> GetMethodWrapper;
        protected Action<object, object> SetMethodWrapper;

        /// <summary>
        /// For internal use only
        /// </summary>
        public Func<object, object> GetMethod;
        /// <summary>
        /// For internal use only
        /// </summary>
        public Action<object, object> SetMethod;

        #region Non-generic Collection Utils  // MOVE

        private static bool GetGenericMethod(string methodName, Type objectType, Type itemType, ref Type lastItemType, ref MethodInfo mi)
        {
            if (mi != null) { l.Trace("More than one recalculation of Collection " + methodName + " method. (OPTIMIZE)"); }
            try
            {
                MethodInfo newMethodInfo = objectType.GetMethod(methodName, new Type[] { itemType });

                if (newMethodInfo == null)
                {
                    l.Error("[BINDING] Could not get Add(" + itemType.ToTypeFullNameSafe() + " ) method on target of type " + objectType.ToTypeFullNameSafe());
                    return false;
                }
                mi = newMethodInfo;
                lastItemType = itemType;
            }
            catch (AmbiguousMatchException)
            {
                l.Error("[BINDING] Could not get Add(" + itemType.ToTypeFullNameSafe() + " ) method on target of type " + objectType.ToTypeFullNameSafe() + ". Got AmbiguousMatch, so this could work by picking a method at random, but this is not implemented.");
                return false;
            }
            return true;
        }

        internal void RemoveRange(object items, Type enumType = null)
        {
            _DoRange("Remove", items, enumType);
        }
        internal void AddRange(object items, Type enumType = null)
        {
            _DoRange("Add", items, enumType);
        }



        private void _DoRange(string methodName, object itemsObject, Type enumType = null)
        {
            if (itemsObject == null) return;

            // REFLECTION OPTIMIZE
            Type lastItemType = null;
            MethodInfo mi = null;

            object collection = this.CachedOrCurrentValue;
            Type collectionType = this.CachedValueType;

            if (collection == null)
            {
                l.Error("Destination collection object is null.  Cannot perform collection binding. (" + LionBinding.ToString() + ")");
                return;
            }

            if (itemsObject.IsBindingProxy())  // itemsObject.GetType().Name.EndsWith("Proxy"))
            {
                // Try to grab the whole List at once -- don't do IEnumerable iteration over the network.

                // REFLECTION OPTIMIZE this lookup
                MethodInfo[] mis = itemsObject.GetType().GetMethods().Where(methodInfo => methodInfo.Name == "ToArray").ToArray();

                if (mis.Length > 0)
                {
                    if (mis.Length > 1)
                    {
                        l.Warn("More than one ToArray method.");
                    }
                    object listObject;
                    listObject = mis[0].Invoke(itemsObject, null);
                    itemsObject = listObject;
                }
                else
                {
                    throw new LionBindingException("Only proxy types with ToArray methods are currently supported. (TODO: support serialization of objects to support IEnumerable, or else determine specific IEnumerable<> type.");
                    //Type enumerableType = typeof(IEnumerable<>).MakeGenericType(Value);

                    //enumerableType.GetMethod("MoveNext");

                    //// TODO 
                    //ArrayList arrayList = new ArrayList();
                    //foreach (object item in (IEnumerable)itemsObject)
                    //{
                    //    arrayList.Add(item);
                    //}
                    //itemsObject = arrayList;
                }
            }

            foreach (var item in (IEnumerable)itemsObject)
            {
                if (item == null)
                {
                    l.Warn("Null item in _DoRange.  Skipping.");
                    continue;
                }
                Type itemType = item.GetType();

                if (lastItemType == null || !lastItemType.IsAssignableFrom(item.GetType()))
                {
                    if (!GetGenericMethod(methodName, collectionType, itemType, ref lastItemType, ref mi))
                    {
                        l.Error("Binding - UNSUPPORTED - No generic add method matching object type: " + itemType.FullName + ".  A conversion may be possible but not implemented.");
                        continue;
                    }
                }
                try
                {
                    l.Trace("DoRange: " + methodName + " " + item.ToString());
                }
                catch { l.Trace("_DoRange trace exception"); }

                mi.Invoke(collection, new object[] { item });
            }


        }

        #endregion


        #region Collection Utils  // MOVE

        //private static void Clear(object collection)
        //{
        //    MethodInfo clearMethod = collection.GetType().GetMethod("Clear");
        //    clearMethod.Invoke(collection, null);
        //}

        public void EnsureCollectionCreatedAndClear()
        {
            if (CachedOrCurrentValue == null)
            {
                CreateCollection();
            }
            else // Exists, so ensure it is cleared
            {
                var clearMethodCopy = ClearMethod;
                if (clearMethodCopy != null)
                {
                    clearMethodCopy.Invoke(Value, null);
                }
                else
                {
                    l.Warn("No Clear method on collection!  Recreating collection.  This may have unintended side effects (lost events, etc)");
                    CreateCollection();
                }
            }
        }

        private void CreateCollection()
        {
            var propertyType = PropertyType;
            if (propertyType == null)
            {
                throw new LionBindingException("Cannot EnsureCreatedAndClear when PropertyType is not known");
            }

            try
            {
                if (!propertyType.IsAbstract && !propertyType.IsInterface)
                {
                    Value = Activator.CreateInstance(propertyType);
                    //l.Trace("Created concrete collection: " + propertyType.Name);
                    return;
                }
            }
            catch (Exception ex)
            {
                l.Trace("Creation of type '" + PropertyType.Name + "'threw exception: " + ex.ToString());
            }

            Value = CollectionInstanceFactory.Default.Create(propertyType);
            //l.Trace("UNTESTED: Created concrete collection for interface: " + propertyType.Name);
        }

        #endregion

        protected MethodInfo ClearMethod {
            get {
                if (clearMethod == null)
                {
                    TryEnsureRetrieved();

                    object val = Value;
                    if (val != null)
                    {
                        clearMethod = Value.GetType().GetMethod("Clear");
                    }
                }
                return clearMethod;
            }
        }
        protected MethodInfo clearMethod;

        #region Property Changed Event Handlers: Set CachedValue to new value and notify.

        void OnPropertyChanged(string propertyName)
        {
            if (!propertyName.Equals(this.PropertyName)) return;

            //CachedValue = this.Value;
            RetrieveValue();
        }

        void notifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals(this.PropertyName)) return;

            RetrieveValue();
            //CachedValue = this.Value;
        }

        void OnPropertyChangedMethod()
        {
            RetrieveValue();
            //CachedValue = this.Value;
        }

        void OnBoolPropertyChangedMethod(bool newValue)
        {
            CachedValue = newValue;
        }

        void OnStringPropertyChangedMethod(string newValue)
        {
            CachedValue = newValue;
        }

        void OnFloatPropertyChangedMethod(float newValue)
        {
            CachedValue = newValue;
        }
        void OnIntPropertyChangedMethod(int newValue)
        {
            CachedValue = newValue;
        }

        #endregion

        private void OnValueChanged(object newValue)
        {
#if TRACE_VALUECHANGED
            l.Trace(this.ToString() + " := " + (newValue ?? "(null)").ToString());
#endif

            //            l.Trace(lionBinding.ToString() + " value changed for " + PropertyName + ": " + (newValue == null ? "null" : newValue.ToString()));
            //if (newValue != this.CachedValue) throw new UnreachableCodeException("New value should be assigned to CachedValue before invoking OnValueChanged");
            clearMethod = null;

            if (NextBinding != null)
            {
                NextBinding.BindingObject = newValue;
            }

            IsBoundToNotifyCollectionChanged = false;
            UpdateCollectionTypes();

            var ev = ValueChanged;
            if (ev != null) ev(newValue);
        }
        public event Action<object> ValueChanged;

        #region Collection Types

        private void UpdatePropertyCollectionTypes()
        {
            var propertyInfo = PropertyInfo;
            if (propertyInfo == null)
            {
                this.PropertyCollectionTypes = null;
                this.PropertyNotifyCollectionTypes = null;
                return;
            }

            var type = propertyInfo.PropertyType;
            this.PropertyCollectionTypes = GetCollectionTypes(type);
            this.PropertyNotifyCollectionTypes = GetNotifyCollectionTypes(type);
        }

        private void UpdateCollectionTypes()
        {
            if (this.NextBinding != null) return;

            object val = CachedValue;
            if (val == null)
            {
                this.ValueCollectionTypes = null;
                this.ValueNotifyCollectionTypes = null;
                return;
            }

            Type type = val.GetType();

            //if(this.IsUpdateWriter) // OPTIMIZE - don't do Collection if this is not a destination
            this.ValueCollectionTypes = GetCollectionTypes(type);

            //if(this.IsUpdateReader)  // OPTIMIZE - don't do NotifyCollectionChanged if this is not a source
            this.ValueNotifyCollectionTypes = GetNotifyCollectionTypes(type);

            if (this.ValueNotifyCollectionTypes.Count > 0)
            {
                IsBoundToNotifyCollectionChanged = true;
            }
        }

        #region PropertyCollectionTypes

        public bool IsPropertyCollection { get { return PropertyCollectionTypes != null && PropertyCollectionTypes.Count > 0; } }

        public List<Type> PropertyCollectionTypes {
            get { return propertyCollectionTypes; }
            set {
                if (propertyCollectionTypes == value) return;

                propertyCollectionTypes = value;

                //var ev = ValueCollectionTypesChanged; if (ev != null) ev();
            }
        }
        private List<Type> propertyCollectionTypes;

        #endregion

        #region PropertyNotifyCollectionTypes

        public List<Type> PropertyNotifyCollectionTypes {
            get { return propertyNotifyCollectionTypes; }
            set {
                if (propertyNotifyCollectionTypes == value) return;

                propertyNotifyCollectionTypes = value;

                var ev = PropertyNotifyCollectionTypesChanged; if (ev != null) ev();
            }
        }
        private List<Type> propertyNotifyCollectionTypes;

        public event Action PropertyNotifyCollectionTypesChanged;

        #endregion

        #region ValueCollectionTypes

        public bool IsValueCollection { get { return ValueCollectionTypes != null && ValueCollectionTypes.Count > 0; } }

        public List<Type> ValueCollectionTypes {
            get { return valueCollectionTypes; }
            set {
                if (valueCollectionTypes == value) return;

                valueCollectionTypes = value;

                var ev = ValueCollectionTypesChanged; if (ev != null) ev();
            }
        }
        private List<Type> valueCollectionTypes;

        public event Action ValueCollectionTypesChanged;

        #endregion

        public bool IsEnumerable {
            get {
                return
                    (PropertyCollectionTypes != null && PropertyCollectionTypes.Any()) ||
                    ValueEnumerableTypes != null && ValueEnumerableTypes.Any();
            }
        }

        public List<Type> ValueEnumerableTypes {
            get { return ValueNotifyCollectionTypes; }  // LIMITATION TODO - Support Enumerable for one-time collection bindings
        }

        //public bool IsNotifyingCollection { get { return ValueNotifyCollectionTypes != null && ValueNotifyCollectionTypes.Count > 0; } } // UNUSED

        public List<Type> ValueNotifyCollectionTypes {
            get { return valueNotifyCollectionTypes; }
            set {
                if (valueNotifyCollectionTypes == value) return;

                valueNotifyCollectionTypes = value;

                var ev = ValueNotifyCollectionTypesChanged; if (ev != null) ev();
            }
        }
        private List<Type> valueNotifyCollectionTypes;

        public event Action ValueNotifyCollectionTypesChanged;

        #region (Static) Collection Types

        private static Dictionary<Type, List<Type>> collectionTypesByType = new Dictionary<Type, List<Type>>();
        private static object collectionTypesByTypeLock = new object();
        private static Dictionary<Type, List<Type>> notifyCollectionTypesByType = new Dictionary<Type, List<Type>>();
        private static object notifyCollectionTypesByTypeLock = new object();

        private static List<Type> GetCollectionTypes(Type type)
        {
            lock (collectionTypesByTypeLock)
            {
                List<Type> types;
                if (collectionTypesByType.TryGetValue(type, out types))
                {
                    // Use cached value
                    return types;
                }

                types = new List<Type>();

                var typesToCheck = new List<Type>();
                typesToCheck.Add(type);
                foreach (Type interfaceType in type.GetInterfaces())
                {
                    typesToCheck.Add(interfaceType);
                }

                //foreach (Type interfaceType in new Type[] { type }.Concat(type.GetInterfaces())) // pre-AOT
                foreach (Type interfaceType in typesToCheck)
                {
                    if (!interfaceType.IsGenericType) continue;
                    Type genericType = interfaceType.GetGenericTypeDefinition();
                    if (genericType == typeof(ICollection<>))
                    {
                        //types.Add(interfaceType.GetGenericArguments()[0]);
                        types.Add(interfaceType);
                    }
                }

                collectionTypesByType.Add(type, types);
                return types;
            }
            // TODO NEXT: Grab the new collection logic from LionRpcInterceptor and put it here, to detect collections from interfaces
        }

        private static List<Type> GetNotifyCollectionTypes(Type type)
        {
            lock (notifyCollectionTypesByTypeLock)
            {
                List<Type> types;
                if (notifyCollectionTypesByType.TryGetValue(type, out types))
                {
                    return types;
                }

                types = new List<Type>();

                var typesToCheck = new List<Type>();
                typesToCheck.Add(type);
                foreach (Type interfaceType in type.GetInterfaces())
                {
                    typesToCheck.Add(interfaceType);
                }

                //foreach (Type interfaceType in new Type[] { type }.Concat(type.GetInterfaces())) // pre-AOT
                foreach (Type interfaceType in typesToCheck)
                {
                    if (!interfaceType.IsGenericType) continue;
                    Type genericType = interfaceType.GetGenericTypeDefinition();
                    if (genericType == typeof(INotifyCollectionChanged<>))
                    {
                        types.Add(interfaceType);
                    }
                }

                notifyCollectionTypesByType.Add(type, types);
                return types;
            }
        }

        #endregion

        #endregion


        public void Detach()
        {
            IsBoundToNotifyCollectionChanged = false;

            if (bindingObject != null)
            {
#if AOT
                l.Warn("TODO: Detach code for AOT");
#endif
                if (isBoundToMultiTypeMethod)
                {
                    //IExtendableMultiTyped multiTyped = bindingObject as IExtendableMultiTyped;
                    var notifyMultiTypeChanged = bindingObject as INotifyMultiTypeChanged;
                    if (notifyMultiTypeChanged != null)
                    {
                        notifyMultiTypeChanged.RemoveTypeHandler(MultiTypeType, new Action<IReadOnlyMultiTyped, Type>(MultiTypeObjectChanged));
                        isBoundToMultiTypeMethod = false;
                    }
                    else
                    {
                        isBoundToMultiTypeMethod = false;
                        l.Error("UNEXPECTED - isBoundToMultiTypeMethod was true but bindingObject is not INotifyMultiTypeChanged");
                    }
                }
                else if (isBoundToSMultiTypeMethod)
                {
                    var notifySMultiTypeChanged = bindingObject as SNotifyMultiTypeChanged;
                    if (notifySMultiTypeChanged != null)
                    {
                        notifySMultiTypeChanged.RemoveTypeHandler(MultiTypeType, new Action<SReadOnlyMultiTypedEx, Type>(SMultiTypeObjectChanged));
                        isBoundToSMultiTypeMethod = false;
                    }
                    else
                    {
                        isBoundToMultiTypeMethod = false;
                        l.Error("UNEXPECTED - isBoundToSMultiTypeMethod was true but bindingObject is not SNotifyMultiTypeChanged");
                    }
                }
                else if (isBoundToChangedMethod)
                {
                    if (PropertyChangedEventInfo.EventHandlerType == typeof(Action))
                    {
                        PropertyChangedEventInfo.RemoveEventHandler(bindingObject, new Action(OnPropertyChangedMethod));
                        isBoundToChangedMethod = false;
                    }
                    else
                    {
                        throw new LionBindingException("Unexpected internal error: isBoundToChangedMethod is true but  (PropertyChangedEventInfo.EventHandlerType == typeof(Action))");
                    }
                }
                else if (isBoundToBoolChangedMethod)
                {
                    PropertyChangedEventInfo.RemoveEventHandler(bindingObject, new Action<bool>(OnBoolPropertyChangedMethod));
                    isBoundToBoolChangedMethod = false;
                }
                else if (isBoundToStringChangedMethod)
                {
                    PropertyChangedEventInfo.RemoveEventHandler(bindingObject, new Action<string>(OnStringPropertyChangedMethod));
                    isBoundToStringChangedMethod = false;
                }
                else if (isBoundToFloatChangedMethod)
                {
                    PropertyChangedEventInfo.RemoveEventHandler(bindingObject, new Action<float>(OnFloatPropertyChangedMethod));
                    isBoundToFloatChangedMethod = false;
                }
                else if (isBoundToIntChangedMethod)
                {
                    PropertyChangedEventInfo.RemoveEventHandler(bindingObject, new Action<int>(OnIntPropertyChangedMethod));
                    isBoundToIntChangedMethod = false;
                }
                else if (isBoundToNotifyPropertyChanged)
                {
                    INotifyPropertyChanged notifyPropertyChanged = bindingObject as INotifyPropertyChanged;
                    notifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(notifyPropertyChanged_PropertyChanged);
                    isBoundToNotifyPropertyChanged = false;
                }
                else if (isBoundToPropertyChanged)
                {
                    IPropertyChanged propertyChanged = bindingObject as IPropertyChanged;
                    propertyChanged.PropertyValueChanged -= new Action<string>(OnPropertyChanged);
                    isBoundToPropertyChanged = false;
                }
                else
                {
                    // Nothing to detach from
                    // OLD - this is not a problem - throw new UnreachableCodeException("Detach called when not known to be bound.");
                }

                #region TODO: Only do this on Dispose:
                ILionBindingHost lionBindingHost = bindingObject as ILionBindingHost;
                if (lionBindingHost != null)
                {
                    lionBindingHost.Bindings.Remove(this);
                }
                else
                {
                    lock (bindingsLock)
                    {
                        Bindings.Remove(this);
                    }
                }
                #endregion
            }

            isBound = false;

            if (NextBinding != null)
            {
                NextBinding.Detach();
            }

            CachedValueToDefault();

        }

        internal bool TryEnsureRetrieved()
        {
            if (!hasRetrieved)
            {
                RetrieveValue();
            }
            return hasRetrieved;
        }

        internal void RetrieveValue()
        {
            if (BindingObject == null) return;
            object val;

            if (!IsRetrieveEnabled)
            {
                if (EnforceIsRetrieveEnabled)
                {
                    l.Trace("TEMP - RetrieveValue invoked when IsRetrieveEnabled == false.  Ignoring.");
                    return;
                }
                else
                {
                    if (!IsPropertyCollection)
                    {
                        // FIXME: Enable this warning and fix it
                        //l.Warn("Warning: RetrieveValue invoked when IsRetrieveEnabled == false and !IsPropertyCollection.  " + this.LionBinding.ToString() + Environment.NewLine + Environment.StackTrace);
                    }
                }
            }

            if (GetMethod == null) { l.Warn("Get Value: No GetMethod"); val = null; }
            else if (GetMethodWrapper == null) { l.Warn("Get Value: No GetMethodWrapper"); val = null; }
            else
            {
                val = this.GetMethodWrapper(BindingObject);
                hasRetrieved = true;
            }
        }

        public object Value {
            get {
                if (!isBound // If not bound to a change event, then retrieve every time.
                    || !hasRetrieved // If value hasn't been retrieved yet (or gotten via event), retrieve it.
                                     //|| _cachedValue == null // HACK 
                                     //|| 
                                     //|| true // HACK
                    )
                {
                    RetrieveValue();
#if TRACE_RETRIEVE
                    l.Trace("R " + this.ToString() + " Retrieving value. ");
#endif
                }

                return CachedValue;
            }
            set {
                if (SetMethod == null) { l.Warn("Get Value: No SetMethod"); return; }
                if (SetMethodWrapper == null) { l.Warn("Get Value: No SetMethodWrapper"); return; }

                try
                {
                    this.SetMethodWrapper(BindingObject, value);
                }
                catch (Exception ex)
                {
                    if (this.LionBinding.ThrowOnSetException)
                    {
                        l.Warn(this.ToString() + ": SetMethodWrapper threw exception (rethrowing): " + ex);
                        throw;
                    }
                    l.Error(this.ToString() + ": SetMethodWrapper threw exception (ignoring): " + ex);
                }

                if (NotifyOnLocalSet)
                {
                    this.CachedValue = value;
                }
                else
                {
                    SetCachedValue(value, false);
                }
            }
        }

        public event Action<LionBindingNode, Exception> ExceptionThrown;

        public object CachedOrCurrentValue // REVIEW - things changed, this may no longer make sense, or things may need clarification
        {
            get {
                TryEnsureRetrieved();
                if (CachedValue != null) return CachedValue;
                return Value;
            }
        }



        internal object CachedValue {
            get {
                return _cachedValue;
            }
            private set {
                SetCachedValue(value);
            }
        }
        private object _cachedValue;

        private void SetCachedValue(object value, bool raiseEvents = true)
        {
            hasRetrieved = true; // assuming this is called by notification methods
            if (_cachedValue == value) return;

            if (value != null && value.GetType().Name.Contains("INotifying"))
            {
                l.Trace("binding cached value: " + value.ToString());
            }
            _cachedValue = value;
            UpdateCachedValueType();

            if (raiseEvents) OnValueChanged(_cachedValue);
        }

        private bool hasRetrieved = false;

        private void CachedValueToDefault() // CODE DUPLICATION - mirrors set accessor
        {
            hasRetrieved = false; // assuming this is called by notification methods

            _cachedValue = GetDefault(this.PropertyType ?? typeof(object));
            UpdateCachedValueType();

            OnValueChanged(_cachedValue);
        }

        private Type CachedValueType {
            get {
                return cachedValueType;
            }
            set {
                if (value == cachedValueType) return;
                cachedValueType = value;
                OnCachedValueTypeChanged();
            }
        }
        private Type cachedValueType;

        private void OnCachedValueTypeChanged()
        {
            UpdateCollectionTypes();
        }

        private void UpdateCachedValueType()
        {
            if (_cachedValue != null)
            {
                CachedValueType = _cachedValue.GetType();
            }
            else
            {
                CachedValueType = null;
            }
        }

        private static ILogger l = Log.Get();

        #region Collection Support


        public Type IsBoundToNotifyCollectionChangedType {
            get {
                return isBoundToNotifyCollectionChangedType;
            }
            private set {
                isBoundToNotifyCollectionChangedType = value;
            }
        }
        private Type isBoundToNotifyCollectionChangedType;

        private class HandlerClass<T> : IDisposable
        {
            LionBindingNode lionBindingNode;
            INotifyCollectionChanged<T> val;

            public HandlerClass(LionBindingNode lionBindingNode, object val)
            {
                this.lionBindingNode = lionBindingNode;
                this.val = (INotifyCollectionChanged<T>)val;
#if !AOT
                this.val.CollectionChanged += new NotifyCollectionChangedHandler<T>(OnCollectionChanged);
#endif
            }

            public void OnCollectionChanged(INotifyCollectionChangedEventArgs<T> e)
            {
                //l.Info("HandlerClass<T>.NotifyCollectionChangedHandler");
                if (!this.lionBindingNode.LionBinding.BindToCollectionEvents) return; // RECENTCHANGE
                lionBindingNode.OnCollectionChanged(e);
            }

            public void Dispose()
            {
#if !AOT
                this.val.CollectionChanged -= new NotifyCollectionChangedHandler<T>(OnCollectionChanged);
#endif
            }
        }

        private List<IDisposable> collectionChangedHandlers = new List<IDisposable>();

        public bool IsBoundToNotifyCollectionChanged {
            get { return isBoundToNotifyCollectionChanged; }
            set {
                if (isBoundToNotifyCollectionChanged == value) return;

                bool oldValue = isBoundToNotifyCollectionChanged;
                try
                {
                    if (ValueNotifyCollectionTypes == null || this.ValueNotifyCollectionTypes.Count == 0) value = false;

                    if (value)
                    {
                        if (NextBinding != null) return;

                        object val = this.CachedValue;
                        if (val == null) return;

                        if (this.ValueNotifyCollectionTypes.Count > 1)
                        {
                            l.Warn("UNEXPECTED / Unsupported: ValueNotifyCollectionTypes.Count > 1.  Choosing only one interface arbitrarily.");
                        }

                        foreach (Type collectionType in this.ValueNotifyCollectionTypes)
                        {
                            // Hall of mirrors:

                            //Type notifyArgsType = typeof(NotifyCollectionChangedEventArgs<>).MakeGenericType(collectionType);
                            //Type notifyType = typeof(INotifyCollectionChanged<>).MakeGenericType(collectionType);
                            //IsBoundToNotifyCollectionChangedType = notifyType;                            

                            Type handlerType = typeof(HandlerClass<>).MakeGenericType(collectionType.GetGenericArguments()[0]);
                            IDisposable handlerObject = (IDisposable)Activator.CreateInstance(handlerType, this, val);
                            ////MethodInfo handlerMethodInfo = handlerType.GetMethod("OnCollectionChanged");

                            //Type notifyDelegateType = typeof(NotifyCollectionChangedHandler<>).MakeGenericType(collectionType);

                            ////Delegate del = (Delegate)Activator.CreateInstance(notifyDelegateType, handlerMethodInfo.MethodHandle);
                            //Delegate del = (Delegate)Activator.CreateInstance(notifyDelegateType, handlerObject, "OnCollectionChanged");
                            //EventInfo eventInfo = IsBoundToNotifyCollectionChangedType.GetEvent("CollectionChanged");
                            //eventInfo.AddEventHandler(val, del);

                            collectionChangedHandlers.Add(handlerObject);
                            //break;
                        }
                        //EventInfo mi = IsBoundToNotifyCollectionChangedType.GetEvent("CollectionChanged");                        

                        //INotifyCollectionChanged ncc = this.Value as INotifyCollectionChanged;
                        //if (ncc == null) return;

                        //ncc.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);

                        isBoundToNotifyCollectionChanged = value;
                    }
                    else
                    {
                        isBoundToNotifyCollectionChanged = value;

                        foreach (var handler in collectionChangedHandlers.ToArray())
                        {
                            handler.Dispose();
                        }
                        collectionChangedHandlers.Clear();

                        //if (NextBinding != null) return;
                        //if (bindingObject == null) return;

                        //INotifyCollectionChanged ncc = this.Value as INotifyCollectionChanged;
                        //if (ncc == null) return;
                        //ncc.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);
                    }
                }
                finally
                {
                    //if (isBoundToNotifyCollectionChanged != oldValue)
                    //{
                    //    var ev = IsBoundToNotifyCollectionChangedChanged;
                    //    if (ev != null) ev();
                    //}
                }
            }
        }
        private bool isBoundToNotifyCollectionChanged;

        //public event Action IsBoundToNotifyCollectionChangedChanged;

        //private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    var ev = CollectionChanged;
        //    if (ev != null) ev(sender, e);
        //}

        private void OnCollectionChanged<CollectionType>(INotifyCollectionChangedEventArgs<CollectionType> args) => CollectionChanged?.Invoke(CachedValue, args.ToNonGeneric());

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        public override string ToString()
        {
            try
            {
                var lb = LionBinding; if (lb == null) return base.ToString();
                return lb.ToString() + "-(" + (this.propertyName ?? "null") + ")";
            }
            catch
            {
                return "{LionBindingNode (ToString threw exception)}";
            }
        }

        public void Dispose()
        {
            Detach();
        }
    }
    public static class LionBindingExtensions
    {
        public static bool IsBindingProxy(this object itemsObject) => itemsObject.GetType().Name.EndsWith("Proxy");
    }
}
