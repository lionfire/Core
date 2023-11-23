using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace LionFire.Vos;

public interface IVobReference<out TValue> : IVobReference, IReference<TValue>
{
}

// vob://^archive/c/temp/testzip.zip
// VobPersisterProvider > ProviderType: ArchivePersisterProvider > DefaultPersister[".zip"] > SharpZipLibPersister

// vob+extend://^archive/c/temp/testzip.zip
// vob+extend://^zip/c/temp/testzip.zip
// vob+extend://^SharpZipLib/c/temp/testzip.zip



public interface IVobExtensionReference : IReference, IReferencable<IVobExtensionReference>, IPersisterReference
{
    IVobReference VobReference { get; set; }
}

public interface IVobReference : IReference, ITypedReference, IReferencable<IVobReference>, IPersisterReference
{
    IVobReference<T> ForType<T>();

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

    IVobReference this[string childPath]
    {
        get;
    }
}

public static class IVobReferenceExtensions
{
    public static string RootName(this IVobReference vobReference) => vobReference.Persister ?? "";

    public static string Filter(this IVobReference vobReference, string filterName)
        => vobReference.Filters?.Where(f => f.Key == filterName).Select(kvp => kvp.Value).Aggregate((x, y) => $"{x},{y}");

    public static IEnumerable<string> Filters(this IVobReference vobReference, string filterName)
        => vobReference.Filters?.Where(f => f.Key == filterName).Select(kvp => kvp.Value) ?? Enumerable.Empty<string>();

    public static void AppendFilterKey(this IVobReference vobReference, string filterName, string prefix, StringBuilder sb)
    {
        if (vobReference.Filters == null) return;
        bool isFirst = true;
        foreach (var kvp in vobReference.Filters)
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