using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;

namespace LionFire.Persistence
{
    public struct PersistenceResult : IPersistenceResult
    {
        public object Error { get; set; }

        public PersistenceResultFlags Flags { get; set; }

        public static readonly PersistenceResult Success = new PersistenceResult { Flags = PersistenceResultFlags.Success };
        public static readonly PersistenceResult NotFound = new PersistenceResult { Flags = PersistenceResultFlags.NotFound };
        public static readonly PersistenceResult PreviewFail = new PersistenceResult { Flags = PersistenceResultFlags.PreviewFail };
        public static readonly PersistenceResult PreviewSuccess = new PersistenceResult { Flags = PersistenceResultFlags.PreviewSuccess };
    }

    //public struct PersistenceResult<TObject>
    //{
    //    public TObject Object { get; set; }

    //    public bool IsSuccess { get; set; }
    //    public string FailReason { get; set; }
    //    public Exception InnerException { get; set; }

    //}

    //public struct OBusPersistenceResult<TObject, TResolutionInfo>
    //{
    //    public TResolutionInfo ResolutionInfo { get; set; }
    //}

    //public class VosResolutionInfo
    //{
    //    public IReference UnderlyingReference { get; set; }
    //}

    //public struct VosPersistenceResult<TObject>
    //{

    //}
}


