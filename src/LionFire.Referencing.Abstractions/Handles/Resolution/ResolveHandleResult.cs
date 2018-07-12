namespace LionFire.Referencing.Resolution
{
    public struct ResolveHandleResult<T>
        where T : class
    {
        public bool IsSuccess { get; set; }
        public T Result { get; set; }

        public object ResolutionDetails { get; set; }

        #region Static

        public static readonly ResolveHandleResult<T> Unsuccessful = new ResolveHandleResult<T>()
        {
            IsSuccess = false,
            Result = null,
        };

        public static readonly ResolveHandleResult<T> Successful = new ResolveHandleResult<T>()
        {
            IsSuccess = true,
            Result = null,
        };

        #endregion
    }
}
