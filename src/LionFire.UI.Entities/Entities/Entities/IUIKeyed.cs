using LionFire.Structures;
using System.Diagnostics;

namespace LionFire.UI
{
    public interface IUIKeyed : IUIObject, IKeyable
    {

        #region Derived

        string Path { get; }

        #endregion
    }

    public static class IUIKeyedExtensions
    {
        public static void RemoveFromParent(this IUIKeyed child)
        {
            if (child == null || child.Parent == null) return;
            if (child.Parent is IUICollection c) { c.Remove(child.Key); }
            else if (child.Parent is IUIParent p) { p.Remove(child); }
            else Debug.WriteLine("Don't know how to RemoveFromParent: " + child.GetType().FullName); // SILENTFAIL
        }
    }
}
