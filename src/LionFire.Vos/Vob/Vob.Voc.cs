#if VOC
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos
{
    public partial class Vob
    {
        #region Main Voc (Voc<object>)
        /// <summary>
        /// Primary Voc for the Vob.  (TODO) Used as the master in-memory list of children
        /// </summary>
        public Voc<object> Voc
        {
            get
            {
                if (voc == null)
                {
                    voc = new Voc<object>(this);
                }
                return voc;
            }
        }
        private Voc<object> voc;
#endregion

#region Vocs (of various types)

#region Fields

        private SortedDictionary<Type, IVoc> vocs = new SortedDictionary<Type, IVoc>(); // TODO: Make ConcurrentDictionary
        private readonly object vocsLock = new object();

#endregion

        public Voc<T> GetVoc<T>()
            where T : class, new()
        {
            lock (vocsLock)
            {
                if (vocs.ContainsKey(typeof(T)))
                {
                    return (Voc<T>)vocs[typeof(T)];
                }
                else
                {
                    var voc = new Voc<T>(this);
                    //try
                    //{
                    vocs.Add(typeof(T), voc);
                    //}
                    //catch (Exception ex)
                    //{
                    //    l.Error("Exception in adding voc.  " + ex.ToString());
                    //    if (vocs.ContainsKey(typeof(T)))
                    //    {
                    //        return (Voc<T>)vocs[typeof(T)];
                    //    }
                    //    else
                    //    {
                    //        throw;
                    //    }
                    //}
                    return voc;
                }
            }
        }

        public IVoc GetVoc(Type T)
        {
            lock (vocsLock)
            {
                if (vocs.ContainsKey(T))
                {
                    return vocs[T];
                }
                else
                {
                    var vh = (IVoc)Activator.CreateInstance(typeof(Voc<>).MakeGenericType(type), this);
                    vocs.Add(type, vh);
                    return vh;
                }
            }
        }

#endregion

    }
}
#endif