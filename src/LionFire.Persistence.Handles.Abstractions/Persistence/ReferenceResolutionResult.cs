using LionFire.Referencing;

namespace LionFire.Persistence.Resolution
{
    // TOTEST:
    //  ResolveAllForRead with 2 serializers configured: json, msgpack or something, and IncludeNonexistant = true

    public class ReferenceResolutionResult
    {
        public ReferenceResolutionResult() { }
        public ReferenceResolutionResult(IReference reference)
        {
            this.Reference = reference;
        }

        public bool? Exists { get; set; }
        public object Handler { get; set; }
        public IReference Reference { get; set; }

    }

}
