#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Referencing;

namespace LionFire.Referencing;

public abstract class ReferenceProviderBase<TConcreteReference> : IReferenceProvider
    where TConcreteReference : IReference
{

    #region Configuration

    public Func<string /* uri */, (TConcreteReference, string? /* error */)> TryParseFunc { get; protected set; }

    #endregion

    #region Lifecycle

    public ReferenceProviderBase(Func<string /* uri */, (TConcreteReference, string? /* error */)>? tryParse = null)
    {
        if(tryParse != null)
        {
            TryParseFunc = tryParse;
        } else
        {
            var mi = typeof(TConcreteReference).GetMethod(nameof(TryParseFunc), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (mi != null)
            {
                TryParseFunc = u => ((TConcreteReference reference, string? error)) mi.Invoke(null, new object[] { u });
            }
            else
            {
                TryParseFunc = u => throw new NotSupportedException();
            }
        }
    }

    #endregion

    public virtual IEnumerable<Type> ReferenceTypes { get { yield return typeof(TConcreteReference); } }

    public virtual IEnumerable<string> UriSchemes { get { yield return UriScheme; } }
    public abstract string UriScheme { get; }

    public (TReference result, string error) TryGetReference<TReference>(string uri) where TReference : IReference
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
    
    public virtual (TConcreteReference reference, string? error) TryGetReference(string uri) => TryParseFunc(uri);


}
