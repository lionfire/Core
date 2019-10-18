using LionFire.Results;

namespace LionFire.ExtensionMethods
{
    public static class ISuccessResultExtensions
    {
        public static bool? IsSuccess(this IResult result) => result is ISuccessResult sr ? sr.IsSuccess : null;
    }
}
