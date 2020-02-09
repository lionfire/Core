#nullable enable
using LionFire.Ontology;

namespace LionFire.Vos
{
    public interface IRootManager : IHas<IRootManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootName"></param>
        /// <returns>The requested IRootVob, or null if IRootVob has not been created.  (use IServiceCollection.AddVosRoot extension method to add additional IRootVobs during application initialization.)</returns>
        IRootVob? Get(string? rootName = null);
    }

    public static class IRootManageExtensions
    {
        // OPTIMIZE - make this a method in IRootManager to avoid traversing to the unnamed IRootVob (which may not exist) and then back to the root manager using the /../rootName/... syntax.
        public static IVob? GetVob(this IRootManager rootManager, string vobPath) => rootManager.Get(null)?[vobPath];
    }
}
