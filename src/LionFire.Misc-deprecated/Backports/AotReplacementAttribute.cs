using System;

namespace LionFire
{
    // Replaces a call to T MethodName<T>(int a, int b) with a call to object MethodName(int a, int b, Type type) plus a cast to type.
    // The method should return T, or else a cast exception will occur at runtime.
    // This attribute does not do anything right now; it is just a marker.  In the future it could be required for the rewriter.

    /// <summary>
    /// Repla
    /// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class AotReplacementAttribute : Attribute
	{
		public AotReplacementAttribute()
		{
		}
	}
}

