using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public IEnumerable<T> AllLayersOfType<T>()
            where T : class
        {
            foreach (var handle in ReadHandles)
            {
                if (!(handle.TryGetObject().Result))
                {
                    continue;
                }

                T obj = handle.Object as T;
                if (obj != null)
                {
                    yield return obj;
                    continue;
                }

                var mt = obj as IReadOnlyMultiTyped;
                if (mt != null)
                {
                    obj = mt.AsType<T>();
                    if (obj != null)
                    {
                        yield return obj;
                        continue;
                    }
                }
            }
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
