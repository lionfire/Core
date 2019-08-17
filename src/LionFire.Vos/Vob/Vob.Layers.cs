using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.MultiTyping;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public partial class Vob
    {
        public T AsTypeOrCreate<T>()
            where T : class, new()
        {
            var first = AsType<T>();
            if (first == null)
            {
                first = new T();
            }
            return first;
        }

        public T AsType<T>()
            where T : class
        {
            var first = AllLayersOfType<T>().FirstOrDefault();
            if (first != null && AllLayersOfType<T>().ElementAtOrDefault(1) != null)
            {
                l.Warn("AsType has more than one match: " + this);
            }
            return first;
        }

        // TODO: Async version?
        public async Task<IEnumerable<T>> AllLayersOfType<T>()
            where T : class
        {
            var results = new List<T>();

            foreach (var handle in ReadHandles)
            {
                if (!(await handle.Get<T>()).HasObject)
                {
                    continue;
                }

                var obj = handle.Object;

                if (obj is T typedObj)
                {
                    results.Add(typedObj);
                    continue;
                }

                if (obj is IReadOnlyMultiTyped mt)
                {
                    var typedObj = mt.AsType<T>();
                    if (typedObj != null)
                    {
                        results.Add(typedObj);
                        continue;
                    }
                }
            }
            return results;
        }
        
        [AotReplacement]
        public object AsType(Type T) => AllLayersOfType(T).FirstOrDefault();

        [AotReplacement] // TODO - support this in Rewriter
        public IEnumerable<object> AllLayersOfType(Type T)
        {
            throw new NotImplementedException("TOPORT");
#if PORT
            foreach (var handle in ReadHandles)
            {
                if (!handle.TryEnsureRetrieved())
                {
                    continue;
                }

                object obj = handle.Object;
                if (T.IsAssignableFrom(obj.GetType()))
                {
                    yield return obj;
                    continue;
                }

                IReadOnlyMultiTyped mt = obj as IReadOnlyMultiTyped;
                if (mt != null)
                {
                    obj = mt.AsType(T);
                    if (obj != null)
                    {
                        yield return obj;
                        continue;
                    }
                }

            }
#endif
        }

        public IEnumerable<object> AllLayers()
        {
            throw new NotImplementedException("TOPORT");
#if PORT
            foreach (var handle in ReadHandles)
            {
                if (!handle.TryEnsureRetrieved())
                {
                    continue;
                }

                object obj = handle.Object;

                var mt = obj as IReadOnlyMultiTyped;
                if (mt != null)
                {
                    foreach (var o in mt.SubTypes)
                    {
                        yield return o;
                    }
                    continue;
                }

                if (obj != null) { yield return obj; }
            }
#endif
        }
        public bool Exists => AllLayers().Any();

#if UNUSED
        public IEnumerable<object> TryEnsureRetrievedAllLayers()
        {
            foreach (var handle in ReadHandles)
            {
                if (!handle.TryEnsureRetrieved())
                {
                    continue;
                }

                yield return handle.Object;
            }
        }
#endif
    }
}
