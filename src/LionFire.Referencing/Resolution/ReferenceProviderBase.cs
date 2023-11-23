#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Referencing;

namespace LionFire.Referencing;

public abstract class ReferenceProviderBaseBase<TConcreteReference> : IReferenceProvider
    where TConcreteReference : IReference
{
    public virtual IEnumerable<Type> ReferenceTypes { get { yield return typeof(TConcreteReference); } }
    public virtual IEnumerable<string> UriSchemes { get { yield return UriScheme; } }
    public abstract string UriScheme { get; }

    public abstract (TConcreteReference reference, string? error) TryGetReference(string uri);

    public abstract (TReference result, string? error) TryGetReference<TReference>(string uri, bool aggregateErrors = false) 
        where TReference : IReference;
}

public abstract class ReferenceProviderBase<TConcreteReference> : ReferenceProviderBaseBase<TConcreteReference> 
    where TConcreteReference : IReference
{
    #region Configuration

    public Func<string /* uri */, (TConcreteReference?, string? /* error */)> TryParseFunc { get; protected set; }

    #endregion

    #region Lifecycle

    public ReferenceProviderBase(Func<string /* uri */, (TConcreteReference, string? /* error */)>? tryParse = null)
    {
        SetTryParse(tryParse);
    }

    protected void SetTryParse(Func<string, (TConcreteReference?, string?)>? tryParse)
    {
        if (tryParse != null)
        {
            TryParseFunc = tryParse;
        }
        else
        {
            var mi = typeof(TConcreteReference).GetMethod(nameof(TryParseFunc), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (mi != null)
            {
                TryParseFunc = u => ((TConcreteReference reference, string? error))mi.Invoke(null, new object[] { u });
            }
            else
            {
                TryParseFunc = u => (default, "Not supported");
            }
        }
    }

    #endregion

    public override (TConcreteReference? reference, string? error) TryGetReference(string uri) => TryParseFunc(uri);

    // OPTIMIZE: may be faster to replace string error with an Action that generates the error string if needed
    public override (TReference result, string? error) TryGetReference<TReference>(string uri, bool aggregateErrors = false)
    {
        if (!typeof(TReference).IsAssignableFrom(typeof(TConcreteReference)))
        {
            return (default, $"Requested type {typeof(TReference).FullName} not supported by this provider");
        }

        if (!uri.StartsWith(UriScheme + ":"))
        {
            return (default, $"Scheme not supported: '{LionUri.GetUriScheme(uri)}'.  Supported schemes: {UriSchemes.Aggregate((x, y) => $"{x}, {y}")}");
        }

        var path = uri.Substring(UriScheme.Length + 1);

        var result = TryGetReference(path);
        return ((TReference)(object)result.reference, result.error);
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TConcreteReference">Must be a Generic type with one type parameter.  What that type is doesn't matter (object recommended)</typeparam>
public abstract class TypedReferenceProviderBase<TConcreteReference> : ReferenceProviderBaseBase<IReference>
    where TConcreteReference : IReference
{
    #region Configuration

    public Func<(string /* uri */, Type /* TValue*/), (TConcreteReference?, string? /* error */)> TryParseFunc { get; protected set; }

    #endregion

    public Type GenericType { get; } = typeof(TConcreteReference).GetGenericTypeDefinition();

    #region Lifecycle

    public TypedReferenceProviderBase(Func<string /* uri */, (IReference, string? /* error */)>? tryParse = null)
    {
        //SetTryParse(tryParse);
    }

    #endregion

    public override (IReference reference, string? error) TryGetReference(string uri) => TryParseFunc((uri, typeof(object)));

    public override (TReference result, string? error) TryGetReference<TReference>(string uri, bool aggregateErrors = false)
    {
        if (!typeof(TReference).IsGenericType || typeof(TReference).GetGenericTypeDefinition() != GenericType)
        {
            return (default, $"Requested type must be a generic of {GenericType}");
        }
        var valueType = typeof(TReference).GetGenericArguments().Single();

        if (!uri.StartsWith(UriScheme + ":"))
        {
            return (default, $"Scheme not supported: '{LionUri.GetUriScheme(uri)}'.  Supported schemes: {UriSchemes.Aggregate((x, y) => $"{x}, {y}")}");
        }

        var path = uri.Substring(UriScheme.Length + 1);

        var result = TryGetReference(path);
        return ((TReference)(object)result.reference, result.error);
    }

}
