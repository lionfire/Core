using LionFire.ObjectBus.FsFacade;
using LionFire.Referencing;

namespace LionFire.ObjectBus
{
    public abstract class WritableFsFacadeOBase<TReference> : WritableOBase<TReference>, IConnectingOBase
        where TReference : class, IReference
    {
        public abstract IFsFacade FsFacade { get; }
        public abstract string ConnectionString { get; set; }

    }

}
