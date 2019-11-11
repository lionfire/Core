using System.Threading.Tasks;

namespace LionFire.Persistence
{

    public interface IDeletable 
    {
        /// <summary>
        /// Returns true if deleted something.
        /// Returns false if delete was attempted but it is known that nothing was deleted.
        /// Returns null if unknown whether something was deleted.
        /// </summary>
        /// <returns>True if something was deleted, false if nothing was found to delete, null if it was not possible to immediately determine whether something was deleted.</returns>
        Task<bool?> Delete();

        void MarkDeleted();

    }
}
