#nullable enable
using LionFire.Ontology;
using LionFire.ExtensionMethods.Acquisition;
using LionFire.Structures;
using System.Collections.Generic;
using System;

namespace LionFire;

public static class IAcquiresExtensions
{
    public static TValue? Acquire2<TNode, TValue>(this TNode parented, int minDepth = 0, int maxDepth = -1)
        where TNode : IAcquires, IParented<TNode>
        where TValue : class
    {
        return ParentedAcquisitionExtensions.Acquire<TNode, TValue>(parented, minDepth, maxDepth);
    }

    public static (TValue value, TNode node)? AcquireWithOwner2<TNode, TValue>(this TNode parented, int minDepth = 0, int maxDepth = -1)
        where TNode : IAcquires, IParented<TNode>
        where TValue : class
    {
        return ParentedAcquisitionExtensions.AcquireWithOwner<TNode, TValue>(parented, minDepth, maxDepth);
    }

    public static IEnumerable<(TValue?, TNode)> GetAcquireEnumerator2<TNode, TValue>(this TNode parented, int minDepth = 0, int maxDepth = -1, bool includeNull = false)
            where TNode : IAcquires, IParented<TNode>
            where TValue : class
    {
        return ParentedAcquisitionExtensions.GetAcquireEnumerator<TNode, TValue>(parented, minDepth, maxDepth, includeNull);
    }

    public static void SetAcquirable<TNode, TValue>(this TNode parented, TValue value)
    {
        throw new NotImplementedException();        
    }
}
