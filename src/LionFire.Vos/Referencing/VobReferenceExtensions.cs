using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace LionFire.Vos;

public static class VobReferenceExtensions
{
    /// <summary>
    /// Shortened form of .ToVobReference, for convenience.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static VobReference V(this string path) => path.ToVobReference();

    public static VobReference<TValue> ToVobReference<TValue>(this string path) => new VobReference<TValue>(path);

    public static VobReference ToVobReference(this string path) => new VobReference(path);
    public static VobReference ToVobReference(this string[] pathChunks) => new VobReference(pathChunks);
    //    public static Vob GetVob(this VobReference vobReference)
    //    {
    //        if (vobReference == null) throw new ArgumentNullException("vobReference");
    //        return Vos.Default[vobReference.Path];
    //    }

    public static VobReference ToVobReference(this IReference reference)
    {
        if (reference is VobReference vr) return vr;
        if (reference.Scheme != VobReference.UriSchemeDefault) throw new ArgumentException("Invalid scheme");
        throw new NotImplementedException();
    }

    public static VobReference GetRelativeOrAbsolutePath(this IVobReference reference, string path)
             => path.StartsWith(LionPath.Separator) ? new VobReference(path) : (VobReference)reference.GetChild(path); // TODO FIXME .. and alternate root name handling

    public static IEnumerable<string> ExtractEnvironmentVariables(this IVobReference vobReference) 
        => vobReference.PathChunks.Where(c => c.StartsWith("$"));
}
