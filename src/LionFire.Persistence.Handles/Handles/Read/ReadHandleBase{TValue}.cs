using System;
using LionFire.Referencing;
using LionFire.Resolves;

namespace LionFire.Persistence.Handles
{
    /// <summary>
    /// Minimal class with no events.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <seealso cref="ReadHandle{TValue}"/>
    public abstract class ReadHandleBase<TValue> : Resolves<IReference, TValue>, IReadHandleBase<TValue>
        //, IReadHandleInvariant<TValue>
    {

        #region Reference

        protected virtual bool IsAllowedReferenceType(Type type) => true;

        [SetOnce]
        public IReference Reference
        {
            get => reference;
            protected set
            {
                if (reference == value)
                {
                    return;
                }

                if (reference != default(IReference))
                {
                    throw new AlreadySetException();
                }

                // OLD: art != null && value != null && !art.Where(type => type.IsAssignableFrom(value.GetType())).Any()
                if (!IsAllowedReferenceType(value.GetType()))
                {
                    throw new ArgumentException("This type does not support IReference types of that type.  See protected IsAllowedReferenceType implementation for allowed types.");
                }

                reference = value;
            }
        }
        protected IReference reference;

        #endregion

        #region Persistence State

        public PersistenceFlags Flags
        {
            get => flags;
            set
            {
                if (flags == value) { return; }

                var oldValue = flags;
                flags = value;

                //OnStateChanged(value, oldValue);
            }
        }
        private PersistenceFlags flags;

        //protected virtual void OnStateChanged(PersistenceFlags newValue, PersistenceFlags oldValue) { }

        #endregion

        #region Derived - Convenience

        public bool IsUpToDate
        {
            get => Flags.HasFlag(PersistenceFlags.UpToDate);
            //protected set => flags.SetFlag(PersistenceFlags.UpToDate, value);
            protected set
            {
                if (value)
                {
                    Flags |= PersistenceFlags.UpToDate;
                }
                else
                {
                    Flags &= ~PersistenceFlags.UpToDate;
                }
            }
        }
        //public bool IsPersisted // REVIEW FIXME
        //{
        //    get => Flags.HasFlag(PersistenceFlags.UpToDate);
        //    set
        //    {
        //        if (value)
        //        {
        //            Flags |= PersistenceFlags.UpToDate;
        //        }
        //        else
        //        {
        //            Flags &= ~PersistenceFlags.UpToDate;
        //        }
        //    }
        //}

        #region Reachable

        public bool? IsReachable
        {
            get => Flags.HasFlag(PersistenceFlags.Reachable) ? true : (Flags.HasFlag(PersistenceFlags.Reachable) ? false : (bool?)null);
            protected set
            {
                // REFACTOR ?
                if (value.HasValue)
                {
                    if (value.Value)
                    {
                        Flags |= PersistenceFlags.Reachable;
                        Flags &= ~PersistenceFlags.Unreachable;
                    }
                    else
                    {
                        Flags |= PersistenceFlags.Unreachable;
                        Flags &= ~PersistenceFlags.Reachable;
                    }
                }
                else
                {
                    Flags &= ~(PersistenceFlags.Reachable | PersistenceFlags.Unreachable);
                }
            }
        }

        #endregion

        #endregion


        #region Construction

        protected ReadHandleBase() { }

        protected ReadHandleBase(IReference input) : base(input) { }

        ///// <summary>
        ///// Do not use this in derived classes that are purely resolve-only and not intended to set an initial value.
        ///// </summary>
        ///// <param name="input"></param>
        ///// <param name="initialValue"></param>
        //protected ReadHandleBase(IReference input, TValue initialValue) : base(input, initialValue)
        //{
        //}

        #endregion
    }


#if false // OLD

        //public  class ReadHandleBaseCovariant<T> : ResolvesBaseCovariant<IReference, T>, IReadHandleEx<T>, IKeyed<string>, IRetrievableImpl<T>, IHas<PersistenceResultFlags>
    //{
    //    public override Task<IResolveResult<T>> ResolveImpl() => throw new NotImplementedException();
    //}


    //public abstract class ReadHandleBase<T> : ResolvesBase<IReference, T>, IReadHandleEx<T>, IRetrievableImpl<T>, IHas<PersistenceResultFlags>, IPersisted
    //{
    //    #region Reference

    //    public IReference Reference
    //    {
    //        get => reference;
    //        set
    //        {
    //            if (reference == value) return;
    //            if (reference != default) throw new AlreadySetException();
    //            reference = value;
    //        }
    //    }
    //    private IReference reference;

    //    #endregion

    //    public PersistenceResultFlags Object => throw new NotImplementedException();
    //}


    public abstract class ReadHandleBase<T> : H<T>, 
        //IReadHandle<T>, 
        INotifyPropertyChanged, IReadHandle, IKeyed<string>
        where T : class
    {
    #region Identity

        [SetOnce]
        public string Key { get; protected set; }

    #endregion

    #region Construction

        public ReadHandleBase() { }
        public ReadHandleBase(string key) { this.Key = key; }

    #endregion

    #region Object

        [Ignore(LionSerializeContext.AllSerialization)]
        public T Object
        {
            get
            {
                if (_object == null)
                {
                    TryResolveObject();
                }
                return _object;
            }
            protected set
            {
                if (object.ReferenceEquals(_object, value)) return;
                //bool resettingObject = _object != null;
                var oldObj = _object;
                _object = value;
                RaiseObjectChanged(oldObj, value);
            }
        }
        protected T _object;
        object IReadHandle.Object => Object;

        protected void RaiseObjectChanged(T oldObj, T newObj)
        {
            ObjectChanged?.Invoke(this, oldObj, _object);

        }
        protected virtual void OnObjectChanged(T oldObj, T newObj)
        {
            RaiseObjectChanged(oldObj, newObj);
            OnPropertyChanged(nameof(Object));
        }

        public event Action<IReadHandle<T>, T, T> ObjectChanged;
        //event Action<IReadHandle, object, object> IReadHandle.ObjectChanged { add => this.ObjectChanged += value; remove => this.ObjectChanged -= value; }

        public bool HasObject => _object != null;


        public void ForgetObject() { Object = null; }

        public abstract Task<bool> TryResolveObject(object persistenceContext = null);

    #endregion

    #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    #endregion

    }
#endif
}

