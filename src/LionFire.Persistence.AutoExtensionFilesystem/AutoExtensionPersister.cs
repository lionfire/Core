using LionFire.Referencing;
using LionFire.Persistence.Persisters;
using System.Collections.Generic;
using LionFire.IO;
using LionFire.Serialization;
using System;

namespace LionFire.Persistence.AutoExtensionFilesystem
{
    public abstract class AutoExtensionPersister<TReference, TOptions, TUnderlyingReference, TUnderlyingPersister>
        : PassthroughPersister<TReference, TOptions, TUnderlyingReference, TUnderlyingPersister>
          where TReference : IReference
        where TOptions : PersistenceOptions
        where TUnderlyingPersister : class, IPersister<TUnderlyingReference>
        where TUnderlyingReference : IReference, IReferencable<TUnderlyingReference>
    {

        protected virtual TUnderlyingReference ConvertReferenceWithPath(TReference reference, string path) => throw new NotImplementedException();

        public virtual async IAsyncEnumerable<string> CandidateReadPaths(TReference reference)
        {
            HashSet<string> results = null;

            //foreach (var selectionResult in PersistenceOptions.SerializationProvider.ResolveStrategies(direction: IODirection.Read))
            foreach (var selectionResult in UnderlyingPersister.SerializationProvider.Strategies)
            {
                foreach (var extension in selectionResult.SupportedExtensions(IODirection.Read))
                {
                    var candidate = ConvertReferenceWithPath(reference, reference.Path + "." + extension);

                    if (results?.Contains(candidate.Key) == true) continue;
                    if ((await (UnderlyingPersister.Exists<object>(candidate))).IsFound() == true)
                    {
                        yield return extension;
                        if (results == null) results = new HashSet<string>();
                        results.Add(candidate.Key);
                    }
                }
            }
        }
    }
}
