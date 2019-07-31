using LionFire.Ontology;
using LionFire.Referencing;

namespace LionFire.ObjectBus.ExtensionlessFs
{
    [Immutable]
    public sealed class RelativePathReference : RelativeReferenceBase
    {        
        public override string Path { get; }

        public RelativePathReference(IReference reference, string newPath) : base(reference)
        {
            this.Path = newPath;
        }        
    }

    //public class ReferenceResolver
    //{
    //    public List<ReferenceResolutionStrategy> Strategies { get; private set; }

    //    public IEnumerable<R<T>> Resolve<T>(IReference r, ResolveOptions options = null)
    //    {
    //        //DependencyContext
    //    }

    //    public Task<T> Get(IReference r)
    //    {

    //    }

    //    public Task<bool> Exists<T>(IReference r)
    //    {

    //    }


    //}
}
