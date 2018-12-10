using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Runtime.Serialization;

namespace LionFire.Structures
{
    /// <summary>
    /// TODO: Figure out how to support .NET 4.5
    /// 
    /// Represents a weak reference, which references an object while still allowing
    /// that object to be reclaimed by garbage collection.
    /// </summary>
    /// <remarks>
    /// Retreived from http://www.codeproject.com/KB/dotnet/generic_WeakReference.aspx under CPOL
    /// </remarks>
    /// <typeparam name="T">The type of the object that is referenced.</typeparam>
    [Serializable]
    public class WeakReferenceX<T>
        : WeakReference where T : class
    {
        /// <summary>
        /// Initializes a new instance of the WeakReference{T} class, referencing
        /// the specified object.
        /// </summary>
        /// <param name="target">The object to reference.</param>
        public WeakReferenceX(T target)
            : base(target)
        { }

        /// <summary>
        /// Initializes a new instance of the WeakReference{T} class, referencing
        /// the specified object and using the specified resurrection tracking.
        /// </summary>
        /// <param name="target">An object to track.</param>
        /// <param name="trackResurrection">Indicates when to stop tracking the object. 
        /// If true, the object is tracked
        /// after finalization; if false, the object is only tracked 
        /// until finalization.</param>
        public WeakReferenceX(T target, bool trackResurrection)
            : base(target, trackResurrection)
        { }

        //protected WeakReferenceX(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{ }

        /// <summary>
        /// Gets or sets the object (the target) referenced by the 
        /// current WeakReference{T} object.
        /// </summary>
        public new T Target
        {
            get
            {
                return (T)base.Target;
            }
            set
            {
                base.Target = value;
            }
        }
    }
}
