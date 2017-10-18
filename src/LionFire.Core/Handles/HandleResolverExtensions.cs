using LionFire.ObjectBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Referencing
{

    public static class HandleResolverExtensions
    {
        public static T ResolveHandles<T>(this T collection)
            where T : ICollection<object>
        {
            collection.ResolveHandlesAsync().GetResultSafe();
            return collection;
        }

        public static async Task<T> ResolveHandlesAsync<T>(this T collection)
            where T : ICollection<object>
        {
            var replacements = new List<object>();
            var removals = new List<object>();

            await Task.WhenAll(collection.OfType<IReadHandle<object>>().Select(async rh => await rh.TryLoadNonNull().ConfigureAwait(false))).ConfigureAwait(false);

            foreach (var component in collection.OfType<IReadHandle<object>>().ToArray())
            {
                if (component.Object == null) throw new ObjectNotFoundException(component);
                try
                {
                    collection.Remove(component);
                    collection.Add(component.Object);
                }
                catch
                {
                    // Try to put things back to the way it was on failure
                    if (!collection.Contains(component) && !collection.Contains(component.Object))
                    {
                        collection.Add(component);
                        // TODO: try/catch for this block, encapsulating outer exception
                    }
                    throw;
                }
            }
            return collection;
        }
    }
}

