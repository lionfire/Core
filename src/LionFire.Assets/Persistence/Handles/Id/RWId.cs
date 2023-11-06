using LionFire.Data.Id;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;

namespace LionFire.Persistence
{
    public interface IIdReadWriteHandle<TValue> : IIdReadWriteHandle, IReferencable<IIdReference<TValue>> { }

    public interface IIdReadWriteHandle : IReferencable<IIdReference>, IReadWriteHandle, IIdReadHandle
    {
    }

    public interface IHId : IIdReadWriteHandle
    {
        // PORTINGGUIDE - Type > TreatAsType
    } // TEMP TOPORT


    public class RWId<TValue> : ReadWriteHandlePassthrough<TValue, IIdReference<TValue>>, IIdReadWriteHandle<TValue>
        where TValue : IIdentified<string>
    {

        #region Construction and Implicit Operators

        public RWId() { }
        public RWId(IReadWriteHandle<TValue> handle) : base(handle) { }
        public RWId(TValue ided) { Reference = new IdReference<TValue>(ided.Id); Value = ided;   }

        public static implicit operator RWId<TValue>(string assetPath) => new RWId<TValue> { Reference = assetPath };
        public static implicit operator RWId<TValue>(TValue asset) => new RWId<TValue>(asset);
        public static implicit operator IdReference<TValue>(RWId<TValue> asset) => asset.Reference;
        public static implicit operator TValue(RWId<TValue> rId) => rId.Value;

        #endregion

        public string IdPath => Reference.Path;
        public new IdReference<TValue> Reference { get => (IdReference<TValue>)base.Reference; set => base.Reference = value; }
        IIdReference IReferencable<IIdReference>.Reference => Reference;


    }
}
