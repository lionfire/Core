using LionFire.ObjectBus;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using LionFire.Structures;

namespace LionFire.Handles
{
    public abstract class ReadHandleBase<T> : IReadHandle<T>, INotifyPropertyChanged, IReadHandle, IKeyed<string>
        where T : class
    {

        [SetOnce]
        public string Key { get; protected set; }

        public ReadHandleBase() { }
        public ReadHandleBase(string key) { this.Key = key; }

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
}