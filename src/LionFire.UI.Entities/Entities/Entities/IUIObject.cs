using LionFire.Ontology;
using System;

namespace LionFire.UI
{
    public interface IUIObject : IParentable<IUIObject>, IParented<IUIObject>
    {
    }


    public static class IUIObjectExtensions
    {
        public static void Add(this IUIObject @obj, IUIObject child)
        {
            if (obj is IUICollection c && child is IUIKeyed k) { c.Add(k); return; }

            if (obj is IUIParent p)
            {
                if (p.Child != null)
                {
                    if (!object.ReferenceEquals(p.Child, child)) { throw new AlreadySetException($"{nameof(obj)} only supports one Child and it is already set."); }
                    return;
                }
                else
                {
                    p.Child = child;
                    return;
                }
            }
            throw new NotSupportedException();
        }
    }
}
