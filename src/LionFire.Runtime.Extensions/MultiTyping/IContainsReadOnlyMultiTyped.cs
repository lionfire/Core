using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{
    public interface IContainsReadOnlyMultiTyped
    {
        IReadonlyMultiTyped MultiTyped { get; }
    }
    public static class IContainsReadOnlyMultiTypedExtensions
    {
        public static T AsType<T>(this IContainsReadOnlyMultiTyped cmt)
            where T : class
        {
            return cmt.MultiTyped.AsType<T>();
        }

    }
}
