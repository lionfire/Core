using LionFire.Referencing;
using System.Linq;

namespace LionFire.Assets
{
    public static class IAssetReferenceExtensions
    {
        public static string AssetName(this IAssetReference ar) => ar.Path.Split(LionPath.SeparatorChar).LastOrDefault();
        public static string AssetDirectory(this IAssetReference ar) => LionPath.GetDirectoryName(ar.Path);
        public static string AssetName(this IReferenceable<IAssetReference> ar) => ar.Reference.AssetName();
        public static string AssetDirectory(this IReferenceable<IAssetReference> ar) => ar.Reference.AssetDirectory();

    }
}
