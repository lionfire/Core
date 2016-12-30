using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Structures
{
    public interface IChanged
    {
        event Action<object> Changed;
    }

    public static class IChangedExtensions
    {
        public static void AttachChangedEventToCollections(this IChanged obj, Action eventInvoker)
        {
            foreach (var pi in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => typeof(INotifyCollectionChanged).IsAssignableFrom(p.PropertyType)))
            {
                var incc = pi.GetValue(obj) as INotifyCollectionChanged;
                incc.CollectionChanged += (s, e) => eventInvoker();
            }
        }
    }

}
