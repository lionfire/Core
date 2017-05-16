#if FUTURE
namespace LionFire.Serialization
{
    public interface IIdentifyingSerializer
    {
        byte[] ExplicitIdentificationMarker { get; }
    }
}

#endif