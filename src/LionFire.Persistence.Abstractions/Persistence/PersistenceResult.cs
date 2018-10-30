using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;

namespace LionFire.Persistence
{
    public struct PersistenceResult<TObject>
    {
        public TObject Object { get; set; }

        public bool IsSuccess { get; set; }
        public string FailReason { get; set; }
        public Exception InnerException { get; set; }

    }

    public struct OBusPersistenceResult<TObject, TResolutionInfo>
    {
        public TResolutionInfo ResolutionInfo { get; set; }
    }

    public class VosResolutionInfo
    {
        public IReference UnderlyingReference { get; set; }
    }

    public struct VosPersistenceResult<TObject>
    {

    }
}


