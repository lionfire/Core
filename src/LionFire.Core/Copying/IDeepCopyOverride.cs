using OX.Copyable;

namespace LionFire.Copying
{
    public interface IDeepCopyOverride
    {
        object Clone(object instance, VisitedGraph visited, object copy, CopyFlags copyFlags);
    }
}
