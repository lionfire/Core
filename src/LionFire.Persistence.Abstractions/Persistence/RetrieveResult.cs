using System;
using LionFire.Referencing;

namespace LionFire.Persistence
{

    public struct RetrieveResult<T> : IRetrieveResult<T>
    {
        public object Error => throw new NotImplementedException();

        public Type[] Types => throw new NotImplementedException();

        public T Object { get; set; }

        public IReference UnderlyingReference { get; set; }

        //public object Details { get; set; }

        public PersistenceResultKind Kind { get; set; }

        IPersistenceResult[] IPersistenceResult.UnderlyingResults => UnderlyingResults;
        public IRetrieveResult<T>[] UnderlyingResults { get; set; }

        #region Static

        public static readonly RetrieveResult<T> NotFound = new RetrieveResult<T>()
        {
            Kind = PersistenceResultKind.Success,
            Object = default,
        };

        public static readonly RetrieveResult<T> Unsuccessful = new RetrieveResult<T>()
        {
            Kind = PersistenceResultKind.Error,
            Object = default,
        };

        public static readonly RetrieveResult<T> NullSuccessful = new RetrieveResult<T>()
        {
            Kind = PersistenceResultKind.Success | PersistenceResultKind.Found,
            Object = default,
        };

        #endregion
    }
}