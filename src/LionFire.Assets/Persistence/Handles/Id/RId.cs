using LionFire.Data.Id;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{
    public interface IIdReadHandle<TValue> : IIdReadHandle, IReferencable<IIdReference<TValue>>
    {
    }
    public interface IIdReadHandle : IReferencable<IIdReference>, IReadHandle
    {
    }

    public interface IHRId : IIdReadHandle // PORTINGGUIDE IHRId > RId
    {
        // PORTINGGUIDE - Type > TreatAsType
    } // TEMP TOPORT

    public class RId<TValue> : ReadHandlePassthrough<TValue, IIdReference<TValue>>, IIdReadHandle<TValue>
        where TValue : IIded<string>
    {

        #region Construction and Implicit Operators

        public static implicit operator RId<TValue>(string id) => new RId<TValue> { Reference = new IdReference<TValue>(id) };
        public static implicit operator RId<TValue>(TValue ided) => new RId<TValue> { Reference = ided.Id, Value = ided };
        public static implicit operator RId<TValue>(RWId<TValue> asset) => new RId<TValue>(asset.ReadWriteHandle); // TOFLYWEIGHT
        public static implicit operator IdReference<TValue>(RId<TValue> asset) => asset.Reference; 
        public static implicit operator TValue(RId<TValue> rId) => rId.Value;

        public RId() { }
        public RId(IReadHandle<TValue> handle) : base(handle) { }

        #endregion
        
        public string IdPath => Reference.Path;
        public new IdReference<TValue> Reference { get => (IdReference<TValue>)base.Reference; set => base.Reference = value; }

        IIdReference IReferencable<IIdReference>.Reference => Reference;

        public static RId<TValue> Get(string assetPath)
            => assetPath;

        public override string ToString() => Reference.ToString();
    }

    //public static class RId
    //{
    //    public static RId<TValue> Get<TValue>(string assetPath)
    //        where TValue : IId<string> => assetPath;
    //}

    //public static class RIdExtensions
    //{
    //    // UNUSED UNNEEDED?
    //    public static RId<TValue> ToRId<TValue>(this IReadHandle<TValue> readHandle)
    //        where TValue : IId<TValue>
    //        => new RId<TValue>(readHandle);

    //    public static RId<TValue> ToRId<TValue>(this string assetPath)
    //     where TValue : IId<TValue>
    //     => assetPath;
    //}
}
