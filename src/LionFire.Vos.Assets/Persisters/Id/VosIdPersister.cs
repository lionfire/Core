using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Assets;
using LionFire.Serialization;
using LionFire.Data.Id;

namespace LionFire.Vos.Id.Persisters
{
    public class VosIdPersister : PassthroughPersister<IIdReference, VosIdPersisterOptions, IVobReference, VosPersister>, IPersister<IIdReference>
    {
        #region Dependencies

        VosIdPersisterOptions Options { get; }
        IPersisterProvider<IVobReference> VosPersisterProvider { get; }

        #endregion

        #region Cache

        public IVobReference DefaultRoot { get; }

        #endregion

        #region Construction

        public VosIdPersister(VosIdPersisterOptions options, IPersisterProvider<IVobReference> persisterProvider, SerializationOptions serializationOptions) : base(options?.SerializationOptions ?? serializationOptions)
        {
            VosPersisterProvider = persisterProvider;
            Options = options;
            DefaultRoot = Options.DefaultRoot;
        }

        #endregion

        protected override VosPersister GetUnderlyingPersister(IIdReference reference)
            => (VosPersister)VosPersisterProvider.GetPersister((reference as IPersisterReference)?.Persister);

        public string GetTypeKey(Type type) => AssetPaths.GetTypeKey(type);

        public override IVobReference TranslateReferenceForRead(IIdReference reference)
            => DefaultRoot.GetVob()[GetTypeKey(reference.Type)][reference.Path].Reference;

        public override IVobReference TranslateReferenceForWrite(IIdReference reference)
        {
            IVobReference result = null;

            result = Vos.Assets.AssetWriteContext.Current?.WriteLocation[GetTypeKey(reference.Type)][reference.Path];

            if (result == null)
            {
                result = DefaultRoot.GetVob()[GetTypeKey(reference.Type)][reference.Path].Reference;
            }
            return result;
        }
    }
}
