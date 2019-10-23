using System.Threading.Tasks;

namespace LionFire.Results
{
    public static class IResultExtensions
    {
        public static T To<T>(this IResult result) => (T)result; // UNTESTED
        public static Task<T> ToResult<T>(this IResult result) => Task.FromResult((T)result); // UNTESTED
    }
}
