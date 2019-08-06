using LionFire.Referencing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence.Resolution
{

    /// <design>
    /// Type is specified because the object may actually be deserialized and populated in the result, and it is undesired to deserialize other types.
    /// 
    /// Typical use:
    ///  - Read:
    ///     - Try to find an item by name, iterate through possibilities until found.
    ///     - List all possible references for name, for some reason (simply to see all the possibilities)
    ///  - Write:
    ///     - Determine first preferred location to write to.
    ///     - List all possible references for name, for some reason (simply to see all the possibilities)
    /// 
    /// </design>
    public interface IReferenceToReferenceResolver
    {
        /// <summary>
        /// Default implementation: returns the first from ResolveAllForRead.
        /// </summary>
        /// <returns></returns>
        /// <design>
        /// 
        /// </design>
        Task<IEnumerable<ReadResolutionResult<T>>> ResolveAll<T>(IReference reference, ResolveOptions options = null);

        /// <summary>
        /// If multitype exists with the specified type, return only it.
        /// </summary>
        /// <remarks>
        /// For filesystems: if VerifyExists = true (recommended), this has to check for the existance of every possible supported file extension.
        /// FUTURE: Also check for unsupported file extensions, in case two apps support different sets of extensions.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="r"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<IEnumerable<WriteResolutionResult<T>>> ResolveAllForWrite<T>(IReference reference, ResolveOptions options = null);
    }

    public static class ReferenceToReferenceResolverExtensions
    {
        public static async Task<ReadResolutionResult<T>> Resolve<T>(this IReferenceToReferenceResolver resolver, IReference reference, ResolveOptions options = null)
        {
            if (options == null)
            {
                options = DefaultResolveOptions;
            } 
            return (await resolver.ResolveAll<T>(reference, options)).FirstOrDefault();
        }
        public static async Task<WriteResolutionResult<T>> ResolveForWrite<T>(this IReferenceToReferenceResolver resolver, IReference reference, ResolveOptions options = null)
        {
            if (options == null)
            {
                options = DefaultResolveForWriteOptions;
            }
            return (await resolver.ResolveAllForWrite<T>(reference, options)).FirstOrDefault();
        }

        private static readonly ResolveOptions DefaultResolveOptions = new ResolveOptions
        {
            VerifyExists = true,
            MaxResults = 1,
        };
        private static readonly ResolveOptions DefaultResolveForWriteOptions = new ResolveOptions
        {
            VerifyExists = true,
            MaxResults = 1,
        };
    }
}
