#nullable enable
using LionFire.Ontology;
using LionFire.Structures.Acquisition;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.ExtensionMethods.Acquisition
{
    //public static class AcquisitionExtensions
    //    where TNode : IParented<TNode>
    //    where TValue : class
    //{
    //}

    public static class ParentedAcquisitionExtensions
    {
        public static TValue? Acquire<TNode, TValue>(this TNode parented, int minDepth = 0, int maxDepth = -1)
            where TNode : IParented<TNode>
            where TValue : class
        {
            return parented.GetAcquireEnumerator<TNode, TValue>(minDepth, maxDepth).FirstOrDefault().Item1;
        }

        public static (TValue? value, TNode node)? AcquireWithOwner<TNode, TValue>(this TNode parented, int minDepth = 0, int maxDepth = -1)
            where TNode : IParented<TNode>
            where TValue : class
        {
            return parented.GetAcquireEnumerator<TNode, TValue>(minDepth, maxDepth).FirstOrDefault();

            //resultNode = parented;

            //int depth = 0;
            //for (int skip = minDepth; skip > 0 && parented != null; skip--)
            //{
            //    parented = parented.Parent;
            //    depth++;
            //}

            //for (TValue node = Acquisitor<TNode, TValue>.GetValue(resultNode); node != null && (maxDepth < 0 || depth <= maxDepth); node = resultNode == null ? null : Acquisitor<TNode, TValue>.GetValue(resultNode))
            //{
            //    if (node != null) return node;

            //    resultNode = resultNode.Parent;
            //    if (resultNode == null) break;
            //    depth++;
            //}
            //return default;
        }

        // REFACTOR: Replace minDepth with parented.SkipParents(depth)
        // REFACTOR: replace loop with GetParentEnumerator?
        public static IEnumerable<(TValue?, TNode)> GetAcquireEnumerator<TNode, TValue>(this TNode? node, int minDepth = 0, int maxDepth = -1, bool includeNull = false)
            where TNode : IParented<TNode>
            where TValue : class
        {
            int depth = 0;
            for (int skip = minDepth; skip > 0 && node != null; skip--)
            {
                node = node.Parent;
                depth++;
            }

            for (TValue? value = node == null ? null : Acquisitor<TNode, TValue>.GetValue(node); node != null && (maxDepth < 0 || depth <= maxDepth); value = node == null ? null : Acquisitor<TNode, TValue>.GetValue(node))
            {
                if (includeNull || value != null) yield return (value, node);

                node = node.Parent;
                if (node == null) break;
                depth++;
            }
        }

        //public static T TryGetOwn<T>(this object o)
        //    where T : class
        //{
        //    if (o is IHas<T> has) { }


        //    if (Acquisition<T>.dict.TryGetValue(o, out var result))
        //    {
        //        return result;
        //    }
        //    return default;
        //}

        //public static T TryGetOwn2<T, TObject>(this TObject o)
        //    where T : class
        //    where TObject : IHas<T>
        //{
        //    return has.Object;

        //    if (Acquisition<T>.dict.TryGetValue(o, out var result))
        //    {
        //        return result;
        //    }
        //    return default;
        //}

        public static void SetAcquirable<TNode, TValue>(this TNode node, TValue value)
            where TNode : IParented<TNode>
            where TValue : class
        {
            Acquisitor<TNode, TValue>.SetValue(node, value);
        }
    }


}