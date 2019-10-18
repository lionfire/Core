using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Results
{
    /// <summary>
    /// Example implementation of ISuccessResult
    /// </summary>
    public struct SuccessResult : ISuccessResult
    {
        public bool? IsSuccess { get; }

        public SuccessResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public static readonly ISuccessResult Success = new SuccessResult(true);
        public static readonly ISuccessResult Fail = new SuccessResult(false);

    }
    public static class IResultExtensions
    {
        public static T To<T>(this IResult result) => (T)result; // UNTESTED
        public static Task<T> ToResult<T>(this IResult result) => Task.FromResult((T)result); // UNTESTED
    }
}
