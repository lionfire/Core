using System;

namespace LionFire.ObjectBus.Resolution
{
    // BRAINSTORM
    public class ResolutionInfo<TOBaseResolutionData> : IResolutionInfo
    {
        public IOBase OBase { get; set; }
        public TOBaseResolutionData OBaseResolutionData { get; set; }
        public virtual bool IsValid => OBase != null;
    }

    // BRAINSTORM
    public class ExpiringOBusResolutionInfo<TOBaseResolutionData> : ResolutionInfo<TOBaseResolutionData>
    {
        public DateTime ResolutionDate { get; set; }
        public TimeSpan ResolutionExpiry { get; set; } // FUTURE: OBase.GetExpiryFor(this);

        public override bool IsValid => base.IsValid && ResolutionDate + ResolutionExpiry > DateTime.UtcNow;
    }
}