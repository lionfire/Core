using System.Runtime.CompilerServices;

namespace LionFire.Structures.Acquisition;

internal static class AcquisitorWeakTable<TNode, TValue>
    //where TNode : IParented<TNode>
    where TValue : class
{
    internal static ConditionalWeakTable<object, TValue> dict = new();
}
