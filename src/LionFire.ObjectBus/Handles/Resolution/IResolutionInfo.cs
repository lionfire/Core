
namespace LionFire.ObjectBus.Resolution
{
    // BRAINSTORM
    public interface IResolutionInfo
    {
        IOBase OBase { get; }
        bool IsValid { get; }
    }
}