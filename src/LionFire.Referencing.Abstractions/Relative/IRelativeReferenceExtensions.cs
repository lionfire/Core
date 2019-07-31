using LionFire.Referencing;

namespace LionFire.ObjectBus.ExtensionlessFs
{
    public static class IRelativeReferenceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="relativePath">Must start with / to access a subdirectory, otherwise it adds to the current name or "filename"</param>
        /// <returns></returns>
        public static IReference WithRelativePath(this IReference r, string relativePath)
        {
            var newPath = r.Path + relativePath;
            return new RelativePathReference(r, newPath);
        }
        public static IReference WithExtension(this IReference r, string extension)
        {
            var newPath = r.Path + "." + extension;
            return new RelativePathReference(r, newPath);
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
