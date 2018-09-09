using System.Threading.Tasks;

namespace LionFire.Referencing.Persistence
{
    public interface IReferenceRetriever
    {
        Task<RetrieveReferenceResult<T>> Retrieve<T>(IReference reference)
            where T : class;
    }
}