namespace LionFire.Referencing.Persistence
{
    public struct RetrieveReferenceResult<T>
        where T : class
    {
        public bool IsSuccess { get; set; }
        public T Result { get; set; }

        public object ResolutionDetails { get; set; }

        #region Static

        public static readonly RetrieveReferenceResult<T> Unsuccessful = new RetrieveReferenceResult<T>()
        {
            IsSuccess = false,
            Result = null,
        };

        public static readonly RetrieveReferenceResult<T> Successful = new RetrieveReferenceResult<T>()
        {
            IsSuccess = true,
            Result = null,
        };

        #endregion
    }
}
