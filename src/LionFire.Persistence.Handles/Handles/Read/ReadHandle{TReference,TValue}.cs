using System;

namespace LionFire.Persistence.Handles
{
    /// <summary>
    /// Adds strongly typed Reference to ReadHandle&lt;TValue&gt;
    /// </summary>
    /// <typeparam name="TReference"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class ReadHandle<TReference, TValue> : ReadHandle<TValue>
    {
        protected override bool IsAllowedReferenceType(Type type) => type == typeof(TReference);
    }
}
