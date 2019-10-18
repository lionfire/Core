using LionFire.Results;

namespace LionFire.Results
{
    public struct ValueSuccessResult<TValue> : IValueResult<TValue>, ISuccessResult
    {
        public TValue Value { get; }
        public bool? IsSuccess { get; }

        public ValueSuccessResult(bool isSuccess, TValue value = default)
        {
            this.Value = value;
            this.IsSuccess = isSuccess;
        }
    }
}
