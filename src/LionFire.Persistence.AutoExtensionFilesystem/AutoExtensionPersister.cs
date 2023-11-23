using LionFire.Referencing;
using LionFire.Persistence.Persisters;
using System.Collections.Generic;
using LionFire.IO;
using LionFire.Data;
using LionFire.Serialization;
using System;
using System.Linq;

namespace LionFire.Persistence.AutoExtensionFilesystem;

public abstract class AutoExtensionPersister<TReference, TOptions, TUnderlyingReference, TUnderlyingPersister>
    : PassthroughPersister<TReference, TOptions, TUnderlyingReference, TUnderlyingPersister>
      where TReference : IReference
    where TOptions : PersistenceOptions
    where TUnderlyingPersister : class, IPersister<TUnderlyingReference>
    where TUnderlyingReference : IReference, IReferencable<TUnderlyingReference>
{

    public AutoExtensionPersister(SerializationOptions serializationOptions) : base(serializationOptions) { }

    protected virtual TUnderlyingReference ConvertReferenceWithPath(TReference reference, string path) => throw new NotImplementedException();

    public virtual async IAsyncEnumerable<string> CandidateReadPaths(TReference reference)
    {
        HashSet<string> results = null;

        var underlyingPersister = GetUnderlyingPersister(reference) as ISerializingPersister;
        var referencePersister = underlyingPersister as IPersister<TUnderlyingReference>;

        //foreach (var selectionResult in PersistenceOptions.SerializationProvider.ResolveStrategies(direction: IODirection.Read))
        foreach (var selectionResult in underlyingPersister.SerializationProvider.Strategies ?? Enumerable.Empty<ISerializationStrategy>())
        {
            foreach (var extension in selectionResult.SupportedExtensions(IODirection.Read))
            {
                var candidate = ConvertReferenceWithPath(reference, reference.Path + "." + extension);

                if (results?.Contains(candidate.Key) == true) continue;
                if ((await (referencePersister.Exists<object>(candidate))).IsFound() == true)
                {
                    yield return extension;
                    if (results == null) results = new HashSet<string>();
                    results.Add(candidate.Key);
                }
            }
        }
    }
}
