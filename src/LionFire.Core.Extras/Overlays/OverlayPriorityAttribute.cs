using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Overlays
{
    public enum OverlayPrecedence
    {
        /// <summary>
        /// Use the default value for the Overlay (by default, the default is Top)
        /// </summary>
        Default = 0,

        /// <summary>
        /// Top of the overlay stack has precedence.  Values lower in the stack can be overridden by higher layers.
        /// </summary>
        Top,

        /// <summary>
        /// Bottom of the overlay stack has precedence and cannot be overridden.
        /// </summary>
        Bottom,
    }

    public enum OverlaySetTarget
    {
        Default = 0,
        Top,
        Bottom,
        Disallowed,
    }


    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class OverlayAttribute : Attribute
    {
        readonly OverlayPrecedence precedence;

        public OverlayAttribute(OverlayPrecedence precedence = OverlayPrecedence.Default)
        {
            this.precedence = precedence;
        }

        public OverlayPrecedence Precedence => precedence;

        public OverlaySetTarget PropertySetTarget
        {
            get;
            set;
        } 
        
    }
}
