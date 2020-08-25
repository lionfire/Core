using LionFire.Persistence;

namespace LionFire.Vos.Id.Persisters
{
    public class VosIdPersisterOptions : PersistenceOptions
    {
        /// <summary>
        /// Location of default Root used to resolve References when the Persister property is null.
        /// </summary>
        public IVobReference DefaultRoot { get; set; } = DefaultRootDefault;

        public static readonly IVobReference DefaultRootDefault = new VobReference("$id");
    }
}
