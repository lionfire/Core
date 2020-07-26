using LionFire.Referencing;
using System.Collections.Generic;

namespace LionFire.Vos.Assets
{

    public class VobAssetReference<TValue> : VobReferenceBase<TValue, VobAssetReference<TValue>>, IVobAssetReference
    {
        #region Scheme

        public new const string UriSchemeDefault = "asset";
        public new const string UriPrefixDefault = "asset:";

        public override IEnumerable<string> AllowedSchemes
        {
            get { yield return UriSchemeDefault; }
        }

        public override string Scheme => UriSchemeDefault;
        public new static readonly string[] UriSchemes = new string[] { UriSchemeDefault };

        #endregion



        #region Channel

        [SetOnce]
        public string Channel
        {
            get => channel;
            set
            {
                if (channel == value) return;
                if (channel != default) throw new AlreadySetException();
                channel = value;
            }
        }
        private string channel;

        #endregion


    }

#if TOOPTIMIZE // OPTIMIZE ideas:
    //  - VosPathChunksReference
    //  - IVobReference
    // TODO: Create this from VobReference
    public class VosChunksReference : VobReferenceBase, IVobReference
    {
        public IVobReference ParentReference { get; }
        public ArraySegment<string> PathChunks { get; set; }
    }

    public class VobReferenceBase<TConcrete> : ReferenceBase<TConcrete>, IVobReference
    {
        // TODO: Get most of this from VobReference
    }
#endif

    // Persister: VobRootName or ::VobRootName#AssetCollectionName or #AssetCollectionName

}
