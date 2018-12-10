namespace LionFire.Persistence
{
    public interface IRetrieveResult<out T>
    {
        /// <summary>
        /// True if operation was successful, even if retrieve did not find anything
        /// </summary>
        bool IsSuccess { get; }
        T Result { get; }
        object Details { get; }
    }

    public struct RetrieveResult<T> : IRetrieveResult<T>
    {

        public bool IsSuccess { get; set; }
        public T Result { get; set; }

        public object Details { get; set; }

        #region Static

        public static readonly RetrieveResult<T> Unsuccessful = new RetrieveResult<T>()
        {
            IsSuccess = false,
            Result = default(T),
        };

        public static readonly RetrieveResult<T> NullSuccessful = new RetrieveResult<T>()
        {
            IsSuccess = true,
            Result = default(T),
        };

        #endregion
    }
}