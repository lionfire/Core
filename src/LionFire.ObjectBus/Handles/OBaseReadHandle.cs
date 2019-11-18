using System;
using System.Threading.Tasks;
using LionFire.ObjectBus;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;
using MorseCode.ITask;

namespace LionFire.ObjectBus.Handles
{
    /// <summary>
    /// New experiment - general purpose concrete read handle, but I may want ObjectBuses to create their own???
    /// </summary>
    /// <remarks>
    /// Handles can act as gatekeeper logic
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class OBaseReadHandle<T> : ReadHandle<T> // TODO: Inherit from or replace with PersisterReadHandle
    {
        public IOBase OBase { get; }

        public OBaseReadHandle(IReference reference, IOBase obase, T handleObject = default) : base(reference, handleObject)
        {
            this.OBase = obase ?? throw new ArgumentNullException(nameof(obase));
        }

        protected override async ITask<IResolveResult<T>> ResolveImpl()
            => await OBase.Get<T>(this.Reference).ConfigureAwait(false);
    }

#if TODO // ? Useful?
    public class LazyOBaseReadHandle<T> : RBase<T>
    {

        //public static R<T> CreateOrGetForReference()
        //{
        //    throw new NotImplementedException("TODO: Resolve OBase, either in ctor or lazily");
        //}

        public OBaseReadHandle(IReference reference, IOBase obase = null) : base(reference)
        {
            this.OBase = obase ?? throw new ArgumentNullException(nameof(obase));
        }
        public IOBase OBase { get; set; }
        //public RH<T> Replacement { get; set; }

        public override async Task<bool> TryRetrieveObject()
        {
            //if (OBase == null) (OBase, Replacement) = DependencyContext.Current.GetService<IOBaseProvider>().TryGetOBase(this);
            //if (OBase == null) { throw new ObjectBusException("Could not determine IOBase to use for this Reference.  Either use another Handle type, set OBase, or make sure IOBaseProvider is available and can resolve References such as this."); }

            var result = await OBase.TryGet(this.Reference).ConfigureAwait(false);
            return result?.IsRetrieved == true;
        }
    }
#endif

}
