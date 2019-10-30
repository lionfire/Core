using System.Threading.Tasks;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Structures;

namespace LionFire.Persistence.Handles
{
    public class WriteOnlyHandle<T> : ResolvesInputBase<IReference>, WO<T>
    {
        public IReference Reference => Key;

        public T Value { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }


        public PersistenceState State => throw new System.NotImplementedException();


        public bool HasValue => throw new System.NotImplementedException();

        T IReadWrapper<T>.Value => throw new System.NotImplementedException();

        T IWriteWrapper<T>.Value { set => throw new System.NotImplementedException(); }

        public void DiscardValue() => throw new System.NotImplementedException();
        public Task<IPutResult> Put() => throw new System.NotImplementedException();
    }
}
