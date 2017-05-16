using System;
using LionFire.Structures;

namespace LionFire.Structures
{
    public class ReadHandle<TKey, TObject> : IReadHandle<TObject>, IKeyed<TKey>
        where TObject : class
    {
        public Func<TKey, TObject> Resolver { get; set; }
        public TKey Key { get; set; }
        public TObject Object
        {
            get
            {
                if (obj == null && Resolver != null)
                {
                    obj = Resolver(Key);
                }
                return obj;
            }
        }
        private TObject obj;

        public void Reset()
        {
            obj = null;
        }

        public bool HasObject => throw new NotImplementedException();
    }
}