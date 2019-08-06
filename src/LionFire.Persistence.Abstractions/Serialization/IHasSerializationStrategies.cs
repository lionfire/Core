using System;
using System.Collections.Generic;

namespace LionFire.Serialization
{
    public interface IHasSerializationStrategies
    {
        IEnumerable<ISerializationStrategy> Strategies { get; }
    }

    public static class IHasSerializationStrategiesExtensions
    {
        public static List<(string extension, ISerializationStrategy strategy, SerializationFormat format)> GetDistinctRankedStrategiesByExtension(this IHasSerializationStrategies hss, Predicate<ISerializationStrategy> filter = null)
        {
            var list = new List<(string, ISerializationStrategy, SerializationFormat)>();
            var extensions = new HashSet<string>();

            foreach (var strategy in hss.Strategies)
            {
                if (filter != null && !filter(strategy)) continue;
                foreach (var format in strategy.Formats)
                {
                    foreach (var extension in format.FileExtensions)
                    {
                        if (extensions.Contains(extension)) continue;
                        extensions.Add(extension);
                        list.Add((extension, strategy, format));
                    }
                }
            }
            return list;
        }

        //public List<object> GetDistinctRankedWriteStrategiesByExtension(this IHasSerializationStrategies hss)
        //{
        //    var x = GetDistinctRankedReadStrategiesByExtension(hss);
        //    x[0].
        //}
    }

}
