using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Structures;
using System;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Dependencies;
using LionFire.Persistence;

namespace LionFire.ObjectBus
{
    public class SingletonOBusBase<TConcrete, TOBase, TReference> : OBusBase<TConcrete, TOBase, TReference>, IDefaultOBaseProvider
    where TConcrete : OBusBase<TConcrete, TOBase, TReference>, IOBus
        where TOBase : class, IOBase
    where TReference : IReference
    {
        public override IOBase SingleOBase => singleOBase.Value;
        private readonly Lazy<IOBase> singleOBase;

        public override IOBase TryGetOBase(IReference reference) => reference is TReference ? this.DefaultOBase : null;
        public override IReadWriteHandle<T> GetReadWriteHandle<T>(TReference reference, T preresolvedValue = default) => throw new NotImplementedException();
        public override IReadHandle<T> GetReadHandle<T>(TReference reference, T preresolvedValue = default) => throw new NotImplementedException();
        public override IReadWriteHandle<T> GetReadWriteHandle<T>(IReference reference, T preresolvedValue = default) => throw new NotImplementedException();
        public override IReadHandle<T> GetReadHandle<T>(IReference reference, T preresolvedValue = default) => throw new NotImplementedException();
        public override (TReference1, string) TryGetReference<TReference1>(string uri) => throw new NotImplementedException();

        public SingletonOBusBase()
        {
            singleOBase = new Lazy<IOBase>(() => ActivatorUtilities.CreateInstance<TOBase>(DependencyContext.Current.ServiceProvider));
        }
    }

    //public abstract class OBaseProvider : IOBaseProvider
    ////where ReferenceType : IReference
    //{
    //    public abstract string[] UriSchemes { get; }

    //    public abstract IOBase GetOBase(string connectionString);

    //    public abstract string UriSchemeDefault
    //    {
    //        get;
    //    }

    //    public abstract IOBase DefaultOBase
    //    {
    //        get;
    //    }

    //    public abstract IEnumerable<IOBase> OBases
    //    {
    //        get;
    //    }

    //    public abstract IEnumerable<IOBase> GetOBases(IReference reference);


    //    public abstract IReference ToReference(string uri);

    //}
}
