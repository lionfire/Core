using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.ExtensionMethods.Resolves;
using LionFire.MultiTyping;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public partial class Vob
    {
        public async Task<T> AsTypeOrCreate<T>()
            where T : class, new()
        {
            var first = await AsType<T>();
            if (first == null)
            {
                first = new T();
            }
            return first;
        }

        public async Task<T> AsType<T>()
            where T : class
        {
            var layers = await AllLayersOfType<T>().ConfigureAwait(false);
            var first = layers.FirstOrDefault();
            if (first != null && layers.ElementAtOrDefault(1) != null)
            {
                l.Warn("AsType has more than one match: " + this);
            }
            return first;
        }

        // TODO: Async version?
        public  Task<IEnumerable<T>> AllLayersOfType<T>()
            //where T : class
        {
            throw new NotImplementedException();
#if TODO
            var results = new List<T>();

            foreach (var handle in ReadHandles)
            {
                if (!(await handle.GetValue()).HasValue)
                {
                    continue;
                }

                var obj = handle.Value;

                if (obj is T typedObj)
                {
                    results.Add(typedObj);
                    continue;
                }

                if (obj is IReadOnlyMultiTyped mt)
                {
                    var asType = mt.AsType<T>();
                    if (asType != null)
                    {
                        results.Add(asType);
                        continue;
                    }
                }
            }
            return results;
#endif
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
