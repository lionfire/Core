using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace LionFire.Vos
{
    public interface IVosReference : IReference, ITypedReference, IReferencable<IVosReference>
    {
        IEnumerable<string> AllowedSchemes { get; }

        #region REVIEW - maybe eliminate these, or eliminate Package and replace Location with MountName

        //string Location { get; set; }
        //string Package { get; set; }

        #endregion

        string[] PathChunks { get; }

        // OLD 
        //bool Equals(object obj);
        //int GetHashCode();
        ImmutableList<KeyValuePair<string, string>> Filters { get; set; }
    }

    public static class IVosReferenceExtensions
    {
        public static string RootName(this IVosReference vosReference) => vosReference.Persister ?? "";

        public static string Filter(this IVosReference vosReference, string filterName)
            => vosReference.Filters?.Where(f => f.Key == filterName).Select(kvp => kvp.Value).Aggregate((x, y) => $"{x},{y}");

        public static IEnumerable<string> Filters(this IVosReference vosReference, string filterName)
            => vosReference.Filters?.Where(f => f.Key == filterName).Select(kvp => kvp.Value) ?? Enumerable.Empty<string>();

        public static void AppendFilterKey(this IVosReference vosReference, string filterName, string prefix, StringBuilder sb)
        {
            if (vosReference.Filters == null) return;
            bool isFirst = true;
            foreach (var kvp in vosReference.Filters)
            {
                if (kvp.Key == filterName)
                {
                    if (isFirst)
                    {
                        sb.Append(prefix);
                        isFirst = false;
                    }
                    else sb.Append(",");
                    sb.Append(kvp.Value);
                }
            }
        }

    }
}