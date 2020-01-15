using LionFire.Referencing;
using System;

namespace LionFire.Vos
{
    public static class VosReferenceExtensions
    {
        public static VosReference ToVosReference(this string path) => new VosReference(path);
        public static VosReference ToVosReference(this string[] pathChunks) => new VosReference(pathChunks);
        //    public static Vob GetVob(this VosReference vosReference)
        //    {
        //        if (vosReference == null) throw new ArgumentNullException("vosReference");
        //        return Vos.Default[vosReference.Path];
        //    }

        public static VosReference ToVosReference(this IReference reference)
        {
            if (reference.Scheme != VosReference.UriSchemeDefault) throw new ArgumentException("Invalid scheme");
            throw new NotImplementedException();
        }

        public static VosReference GetRelativeOrAbsolutePath(this IVosReference reference, string path)
                 => path.StartsWith(LionPath.Separator) ? new VosReference(path) : (VosReference)reference.GetChild(path); // TODO FIXME .. and alternate root name handling

    }
}
