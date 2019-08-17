using LionFire.Persistence;
using LionFire.Referencing;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    public abstract class ObservableHandleDictionary<TKey, THandle, T> : ObservableReadHandleDictionary<TKey, THandle, T>
        where THandle : class, RH<T>, WH<T>
        where T : class
    {

        #region Key translation

        // REVIEW - come up with a clearer name for these methods
        public virtual string KeyToHandleKey(string key)
        {
            return key;
        }
        public virtual string HandleKeyToKey(string key) { return key; }

        #endregion

        public async Task Remove(string key) // RENAME to Unset?
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            await Set((T)null, key);
        }

        [Obsolete]
        public Task<THandle> Add(T obj, string key = null)
        {
            throw new NotSupportedException("Use Set instead");
        }

        /// <summary>
        /// Immediately writes directly to underlying data store.  Does not write to internal collection. 
        /// (FUTURE: write to internal collection, so that a local cache is up to date while a long write is in progress.)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<THandle> Set(T obj, string key = null) 
        {
            //if (!IsWritable) throw new ReadOnlyException("THandle does not implement IWriteHandle<T>");

            THandle handle;
            if (key == null)
            {
                handle = (THandle)Activator.CreateInstance(typeof(THandle));
            }
            else
            {
                handle = (THandle)Activator.CreateInstance(typeof(THandle), KeyToHandleKey(key));
            }

            var writable = (IWriteHandle<T>)handle;
            writable.SetObject(obj);

            if (handle is ICommitable saveable)
            {
                await saveable.Commit().ConfigureAwait(false);
            }
            return handle;
        }
        //public object PersistenceContext { get; set; }
    }
}
