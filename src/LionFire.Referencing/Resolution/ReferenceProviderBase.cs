using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Referencing;

namespace LionFire.Referencing
{
    public abstract class ReferenceProviderBase<TConcreteReference> : IReferenceProvider
        where TConcreteReference : IReference
    {
        public IEnumerable<Type> ReferenceTypes { get { yield return typeof(TConcreteReference); } }

        public IEnumerable<string> UriSchemes { get { yield return UriScheme; } }
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

        public abstract (TConcreteReference reference, string error) TryGetReference(string path);
    }

}
