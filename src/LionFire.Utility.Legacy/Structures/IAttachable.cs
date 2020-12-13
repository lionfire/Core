using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Structures
{
    /// <summary>
    /// Typical use: on IParentable classes, implement OnParentChanged and invoke IsAttached = false/true when the parent is being changed.
    /// </summary>
    public interface IAttachable
    {        
        bool IsAttached { get; set; }
        //void OnDetached();
    }

    public static class AttachableExtensions
    {
        public static void AttachableOnParentChanging(this IAttachable attachable, object oldParent)
        {
            if (oldParent != null)
            {
                attachable.IsAttached = false;
            }
        }
        public static void AttachableOnParentChanged(this IAttachable attachable, object newParent)
        {
            if (newParent != null)
            {
                attachable.IsAttached = true;
            }
        }
    }
}
