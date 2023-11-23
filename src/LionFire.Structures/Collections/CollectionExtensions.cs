// look to MOVE some / all of this to LionFire.Base?  TOPORT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Collections;

namespace LionFire.ExtensionMethods.Collections;

public static class CollectionExtensions
{
    public static void TryAdd<CollectionType>(this ICollection<CollectionType> collection, CollectionType value)
    {
        if (!collection.Contains(value))
        {
            collection.Add(value);
        }
    }

    public static void TryAddRange<CollectionType>(this List<CollectionType> collection, IEnumerable<CollectionType> value)
    {
        collection.AddRange(value.Where(x => !collection.Contains(x)));

        foreach (CollectionType child in
#if AOT
                    (IEnumerable)
#endif
            value)
        {
            if (!collection.Contains(child))
            {
                collection.TryAdd(child);
            }
        }
    }

    public static void TryAddRange<CollectionType>(this ICollection<CollectionType> collection, IEnumerable<CollectionType> value)
    {
        if (value == null) return;

        foreach (CollectionType child in
#if AOT
(IEnumerable)
#endif
            value)
        {
            if (!collection.Contains(child))
            {
                collection.Add(child);
            }
        }
    }


    public static void Shuffle<T>(this IList<T> list) // From http://stackoverflow.com/questions/273313/randomize-a-listt-in-c
    {
        var provider = RandomNumberGenerator.Create();
        //RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

#if NET35  //- TODO REVIEW - doesn't work in NET35?
    public static void TryAddRange<T>(this SortedSet<T> sortedSet, SortedSet<T> additions)
    {
        TryAddRange(sortedSet, additions.Keys);
    }
#endif

    public static void TryAddRange<T>(this SortedSet<T> sortedSet, IEnumerable<T> additions)
    {
        if (additions == null) return;
        foreach (T item in
#if AOT
                    (IEnumerable)
#endif
            additions)
        {
#if NET35
            if(!sortedSet.ContainsKey(item))
#else
            if (!sortedSet.Contains(item))
#endif
            {
                sortedSet.Add(item);
            }
        }
    }
}
