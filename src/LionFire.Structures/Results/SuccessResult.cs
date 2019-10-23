using System;
using System.Collections.Generic;
using System.Text;

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
        public static ISuccessResult Fail => FailResult.Instance;
    }
}
