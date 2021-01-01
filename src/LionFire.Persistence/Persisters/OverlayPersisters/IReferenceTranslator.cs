using LionFire.Referencing;

namespace LionFire.Persistence.Persisters
{
    // REVIEW - Not sure about the niceness of these methods as an API
    public interface IReferenceTranslator<TReference, TUnderlyingReference>
        where TReference : IReference
        where TUnderlyingReference : IReference
    {
        TUnderlyingReference TranslateReferenceForRead(TReference reference);
        TUnderlyingReference TranslateReferenceForWrite(TReference reference);

        TReference ReverseTranslateReference(TUnderlyingReference reference);
    }
}
