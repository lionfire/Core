#nullable enable
using LionFire.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LionFire.Referencing;

public static class IReferenceExtensions
{
    public static string Name(this IReference reference) => LionPath.GetName(reference?.Path);

    public static string PathRelativeTo(this IReference reference, IReference otherReference, bool allowAncestors = false, bool allowAncestorDecendants = false)
    {
        var isAncestor = !reference.Path.StartsWith(otherReference.Path);

        if (isAncestor)
        {
            if (allowAncestors) { throw new NotImplementedException(); }
            else throw new ArgumentException("reference.Path must start with otherReference.Path when allowAncestors is false.");
        }
        else
        {
            return reference.Path.Substring(otherReference.Path.Length);
        }
    }

    public static ITypedReference<T> OfType<T>(this IReference reference) => new TypedReference<T>(reference); // TODO: return IReference instead of this simplistic wrapper

    public static TReference WithPath<TReference>(this TReference reference, string newPath)
        where TReference : IReference
    {
        if (reference is ICloneableReference cr) { return (TReference)cr.CloneWithPath(newPath); }

        var ub = new UriBuilder(reference.Url)
        {
            Path = newPath
        };

        var mi = FromUriMethods[reference.GetType()];
        if (mi != null) { return (TReference)mi.Invoke(null, new object[] { ub.Uri }); }

        throw ThrowUnsupported(reference, nameof(WithPath));
    }

    #region Parent / Ancestor

    public static T GetParent<T>(this T reference, bool nullIfBeyondRoot = false)
        where T : IReference
        => reference.GetAncestor(depth: 1, nullIfBeyondRoot: nullIfBeyondRoot);

    public static T GetAncestor<T>(this T reference, int depth = 1, bool nullIfBeyondRoot = false)
      where T : IReference
    {
        var newPath = LionPath.GetAncestor(reference.Path, depth, nullIfBeyondRoot);
        return newPath == null ? (T)default : reference.WithPath<T>(newPath);
    }

    #endregion

    #region GetChild

    public static T GetChild<T>(this T reference, string subPath)
        where T : IReference
    {
        if (reference is ICloneableReference cr)
        {
            return (T)cr.CloneWithPath(LionPath.Combine(reference.Path, subPath));
        }

        var uri = new Uri(reference.Url);
        var newUri = new Uri(uri, subPath);

        var mi = FromUriMethods[reference.GetType()];
        if (mi != null)
        {
            return (T)mi.Invoke(null, new object[] { newUri });
        }

        throw ThrowUnsupported(reference, nameof(GetChild));
    }

    #endregion

    #region GetChildSubpath

    public static TReference GetChildSubpath<TReference>(this TReference reference, params string[] subPath)
        where TReference : IReference
        => reference.GetChildSubpath((IEnumerable<string>)subPath);

    //[Untested]
    public static TReference GetChildSubpath<TReference>(this TReference reference, IEnumerable<string> subPath)
        where TReference : IReference
    {
        if (reference is ICloneableReference cr)
        {
            return (TReference)cr.CloneWithPath(LionPath.Combine(reference.Path, subPath));
        }

        var uri = new Uri(reference.Url);
        var newUri = new Uri(uri, subPath.Aggregate((x, y) => x + "/" + y));

        var mi = FromUriMethods[reference.GetType()];
        if (mi != null)
        {
            return (TReference)mi.Invoke(null, new object[] { newUri });
        }

        throw ThrowUnsupported(reference, nameof(GetChildSubpath));
    }

    private static NotSupportedException ThrowUnsupported(IReference reference, string methodName) =>
        new NotSupportedException($"To use {methodName}, reference type ${reference.GetType().FullName} must implement ICloneableReference or have a FromUri method accepting a single parameter of type Uri.");

    #endregion

    #region (Private)

    private static readonly ConcurrentDictionaryCache<Type, MethodInfo> FromUriMethods =
        new ConcurrentDictionaryCache<Type, MethodInfo>(type =>
        type.GetMethod("FromUri", new Type[] { typeof(string) })
        ?? type.GetMethod(type.Name, new Type[] { typeof(string) })
        );

#if FUTURE // Also support FromPath instead of 
    private static readonly ConcurrentDictionaryCache<Type, Func<IReference, string, IReference>> FromUriMethods2 =
        new ConcurrentDictionaryCache<Type, Func<IReference, string, IReference>>(type =>
        {
            MethodInfo mi;

            if ((mi = type.GetMethod("FromUri", new Type[] { typeof(string) })) != null)
            {
                return (reference, subPath) =>
                {
                    var uri = new Uri(reference.Key);
                    var newUri = new Uri(uri, subPath);

                    return (IReference)mi.Invoke(null, new object[] { newUri });
                };
            }
            else if ((mi = type.GetMethod("FromStringUri", new Type[] { typeof(string) })) != null)
            {
                return (reference, subPath) =>
                {
                    var uri = new Uri(reference.Key);
                    var newUri = new Uri(uri, subPath);

                    return (IReference)mi.Invoke(null, new object[] { newUri.ToString() });
                };
            }
            else if ((mi = type.GetMethod("FromPath", new Type[] { typeof(string) })) != null)
            {
                return (reference, subPath) =>
                {
                    var uri = new Uri(reference.Key);
                    var newUri = new Uri(uri, subPath);

                    return (IReference)mi.Invoke(null, new object[] { newUri.AbsolutePath });
                };
            }
            else
            {
                return (reference, subPath) =>
                    throw new NotImplementedException($"This reference type ({reference.GetType().Name}) does not implement one of: FromUri FromStringUri FromPath.");
            }
        });
#endif

    #endregion

    public static NotFoundException NotFoundException(this IReference reference)
    {
        return new NotFoundException($"Could not find {reference.ToString()}")
        {
            Reference = reference,
        };
    }

    /// <summary>
    /// Upcast to ITypedReference and return ITypedReference.Type.  Returns null if none available.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reference"></param>
    /// <returns></returns>
    public static Type? ReferenceType(this IReference reference)
    {
        if (reference is ITypedReference tr && tr.Type != null)
        {
            return tr.Type;
        }
        return null;
    }

    public static IReference<TValue> Cast<TValue>(this IReference reference) => reference as IReference<TValue> ?? new ReferenceCast<TValue>(reference ?? throw new ArgumentNullException(nameof(reference)));

    public static IReference Innermost(this IReference r)
    {
        var inner = r;
        while(inner is IReferenceCast rc) {
            inner = rc.InnerReference;
        }
        return inner;
    }
}
