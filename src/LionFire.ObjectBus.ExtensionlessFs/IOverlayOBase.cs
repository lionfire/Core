namespace LionFire.ObjectBus.ExtensionlessFs
{
    public interface IOverlayOBase : IOBase
    {
         IOBase UnderlyingOBase { get; }
    }
}
