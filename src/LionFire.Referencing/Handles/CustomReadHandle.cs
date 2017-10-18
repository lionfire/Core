using System;
using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.Handles
{
    public class CustomReadHandle<TObject> : ReadHandleBase<TObject>, IReadHandle<TObject>
        //, IKeyed<TKey>
        where TObject : class
    {

        public CustomReadHandle() { }
        public CustomReadHandle(string key) : base(key) { }
        public Func<string, TObject> Resolver { get; set; }
        //public TObject Object
        //{
        //    get
        //    {
        //        if (obj == null && Resolver != null)
        //        {
        //            obj = Resolver(Key);
        //        }
        //        return obj;
        //    }
        //    protected set
        //    {
        //        var resettingObject = obj != null;
        //        obj = value;
        //        if (resettingObject) ObjectReset?.Invoke(this);
        //    }
        //}


        public override Task<bool> TryResolveObject(object persistenceContext = null)
        {
            this._object = Resolver(Key);
            return Task.FromResult(_object != null);
        }

    }
}
