using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Types;
using System.Collections;
using LionFire.MultiTyping;

namespace LionFire.Overlays
{
    /// <summary>
    /// A stack of IMultiTyped objects.  Retrievals either override or aggregate depending on the method.
    /// REVIEW: Overlay options, (using OverlayParameters or similar)
    /// </summary>
    public class MultiTypeOverlay : IReadOnlyMultiTyped
    {

        public SortedList<int, IMultiTyped> Objects
        {
            get => objects;
            internal set => objects = value;  // For serialization
        }
        private SortedList<int, IMultiTyped> objects = new SortedList<int, IMultiTyped>(
#if AOT
Singleton<IntComparer>.Instance
#endif
);

        /// <summary>
        /// Returns the last multiType subtype object of the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object this[Type type]
        {
            get
            {
#if AOT
                List<IMultiTyped> list = new List<IMultiTyped>();

                foreach (KeyValuePair<int, IMultiTyped> kvp in objects)
                {
                    list.Add(kvp.Value);
                }
                for (int i = list.Count; i >= 0; i--)
                {
                    object result = list[i][type];
                    if (result != null) return result;
                }
#else
                foreach (var obj in objects.Values.Reverse())
                {
                    object result = obj[type];
                    if (result != null) return result;
                }
#endif
                return null;
            }
        }

        /// <summary>
        /// Returns the last multiType subtype object of the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>

#if !NoGenericMethods
        public T AsType<T>() where T : class
        {
            foreach (var obj in objects.Values.Reverse())
            {
                T result = obj.AsType<T>();
                if (result != null) return result;
            }
            return null;
        }
#endif


        [AotReplacement]
        public object AsType(Type T)
        {
#if AOT
            List<IMultiTyped> list = new List<IMultiTyped>();

            foreach (KeyValuePair<int, IMultiTyped> kvp in objects)
            {
                list.Add(kvp.Value);
            }
            for (int i = list.Count; i >= 0; i--)
            {
                object result = list[i][T];
                if (result != null) return result;
            }
#else

			foreach (var obj in objects.Values.Reverse())
			{
				var result = obj.AsType(T);
				if (result != null) return result;
			}
#endif
            return null;
        }


        /// <summary>
        /// Aggregates all subtypes of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
#if !NoGenericMethods
        public IEnumerable<T> OfType<T>() where T : class
        {
            var results = new List<T>();

#if AOT
            List<IMultiTyped> list = new List<IMultiTyped>();

            foreach (KeyValuePair<int, IMultiTyped> kvp in objects)
            {
                list.Add(kvp.Value);
            }
            for (int i = list.Count; i >= 0; i--)
            {
                T result = (T) list[i][typeof(T)];
                if (result != null) results.Add(result);
            }
#else
            foreach (var obj in objects.Values)
            {
                var result = obj.OfType<T>();
                if (result != null)
                {
                    results.AddRange(result);
                }
            }
#endif
            return results;
        }
#endif
        [AotReplacement]
        public IEnumerable<object> OfType(Type T)
        {
            var results = new List<object>();

            foreach (var obj in objects.Values)
            {
                var result = obj.OfType(T);
                if (result != null)
                {
                    results.AddRange(result);
                }
            }
            return results;
        }

        /// <summary>
        /// Aggregates all subtypes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<object> SubTypes
        {
            get
            {
                List<object> results = new List<object>();

#if AOT
                List<IMultiTyped> list = new List<IMultiTyped>();

                foreach (KeyValuePair<int, IMultiTyped> kvp in objects)
                {
                    list.Add(kvp.Value);
                }
                for (int i = list.Count; i >= 0; i--)
                {
                    IMultiTyped result = list[i];
                    foreach (var type in result.SubTypes)
                    {
                        results.Add(type);
                    }
                }
#else
                foreach (var obj in objects.Values)
                {
                    var result = obj.SubTypes;
                    if (result != null)
                    {
                        results.AddRange(result);

                    }
                }
#endif
                return results;
            }
        }
    }

}
