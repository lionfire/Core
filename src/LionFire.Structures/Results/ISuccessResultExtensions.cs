using LionFire.Results;

namespace LionFire.ExtensionMethods
{

    public static class ISuccessResultExtensions
    {
        public static bool? IsSuccess(this IResult result) => result is ISuccessResult sr ? sr.IsSuccess : null;
    }
}

namespace LionFire.Results
{
    public static class ISuccessResultExtensions
    {
        public static T ValidateSuccess<T>(this T successResult)
            where T : ISuccessResult
        {
            if (successResult.IsSuccess != true) { throw new Exception("Unsuccessful: " + successResult.ToString()); }
            return successResult;
        }
    }
}