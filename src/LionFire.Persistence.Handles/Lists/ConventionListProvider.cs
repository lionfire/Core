#nullable enable
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
  

    public class ConventionListProvider<TReference> : IListProvider<TReference>
         where TReference : IReference
    {
        ITypeResolver TypeResolver { get; }

        public ConventionListProvider(ITypeResolver typeResolver)
        {
            TypeResolver = typeResolver;
        }

        public  Task<IEnumerable<IListing<T>>> List<T>(Type itemType, IPersister<TReference> persister, IReferencable<TReference> referencable, ListFilter? filter = null)
        {
            //var result = (await persister.List(referencable, filter).ConfigureAwait(false)).ThrowIfUnsuccessful().Value;
            throw new NotImplementedException("NEXT - how to do all this?");
            //persister.List(referencable, filter)
        }

        public  Task<IEnumerable<IListing<T>>> List<T>(IPersister<TReference> persister, IReferencable<TReference> referencable, ListFilter? filter = null)
        {
            // Alternate ideas:
            // - Get all "..collection *", to look for "..collection Type=MyType" or "..CollectionType=MyType"

            var rh = referencable.Reference.GetChild("..collection").GetReadHandle<string>();
            var typeName = rh.Value;

            Type? type = TypeResolver.TryResolve(typeName);

            if (type == null) { throw new Exception($"Failed to resolve collection type: '{typeName}'"); }

            return List<T>(type, persister, referencable, filter);
        }

        //public async Task<IEnumerable<string>> List(IPersister<TReference> persister, IReferencable<TReference> referencable, ListFilter? filter = null)
    }
}

