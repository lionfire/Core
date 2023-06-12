using System;
using System.Diagnostics.Metrics;

namespace LionFire;


// Replaces a call to T MethodName<T>(int a, int b) with a call to object MethodName(int a, int b, Type type) plus a cast to type.
// The method should return T, or else a cast exception will occur at runtime.
// This attribute does not do anything right now; it is just a marker.  In the future it could be required for the rewriter.

[Obsolete("AotReplacement is no longer being used. This was for Unity's old AOT limitations.  I think it works better now and this isn't needed.")]
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class AotReplacementAttribute : Attribute
{
    public AotReplacementAttribute()
    {
    }
}

