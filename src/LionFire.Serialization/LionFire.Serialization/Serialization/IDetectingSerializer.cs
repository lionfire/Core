#if FUTURE
namespace LionFire.Serialization
{
    public interface IDetectingSerializer
    {
        bool ImplicitDetect(byte[] bytes);
        bool ExplicitDetect(byte[] bytes);
    }
}

#endif