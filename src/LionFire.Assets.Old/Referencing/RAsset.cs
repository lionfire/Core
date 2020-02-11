using LionFire.ObjectBus;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    // FUTURE - not used yet

    public class RAsset : TypedReferenceBase<RAsset>
    {
        //string Dummy;

        public const string AssetUriScheme = "asset";
        public const string AssetUriPrefix = AssetUriScheme + ":";

        public override string Scheme => AssetUriScheme;

        public override IEnumerable<string> AllowedSchemes => throw new NotImplementedException();

        public override string Key { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public override string Host { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Port { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Path { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #region Construction

        public RAsset()
        { }

        //public RAsset(string assetName)
        //{ }

        //public RAsset(IReference reference)
        //{ }

        #endregion

        public override string ToString() => string.Format("{0}{1}({2})", AssetUriPrefix, Path, Type);
    }
}
