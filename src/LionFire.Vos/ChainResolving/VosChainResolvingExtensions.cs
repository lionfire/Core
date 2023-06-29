using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Data.Gets.ChainResolving;

namespace LionFire.Vos.ChainResolving
{
    public static class VosChainResolvingExtensions
    {

        /// <summary>
        /// Examples:
        ///  - MyClass myObj = vob.ChainResolve("child1", "child2/child3", typeof(MyClass))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vob"></param>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Task<ChainResolveResult<T>> ChainResolve<T>(this IVob vob, object obj, ChainResolveOptions options = null)
        {
            return obj.ChainResolveAsync<T>(vob.Acquire<ChainResolveOptions>() ?? Default);
        }

        public static ChainResolveOptions Default = new ChainResolveOptions
        {
            Parent = ChainResolveOptions.Default,
            Resolvers = new List<ChainResolverWorker>
            {
                new ChainResolverWorker(typeof(IVob), (Func<object, string, object>) ((o,subpath) => ((IVob)o)[subpath])),
                new ChainResolverWorker(typeof(IVob), (Func<object, Type, object>) ((o,type) => ((IVob)o).GetReadHandle(type))),
            }
        };
    }
}
