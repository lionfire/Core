using LionFire.Persistence;
using LionFire.Structures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using LionFire.Referencing;
using LionFire.Persistence.Handles;

namespace LionFire.ObjectBus
{
    public static class HandleAutoSaveManagerExtensions
    {
        public static void SetAutosave(this IHandleBase obj, bool enabled = true)
        {
            if (enabled)
            {
                HandleAutoSaveManager.Instance.Register(obj);
            }
            else
            {
                HandleAutoSaveManager.Instance.Unregister(obj);
            }

        }
    }

    public class HandleAutoSaveManager
    {
        public static HandleAutoSaveManager Instance { get { return Singleton<HandleAutoSaveManager>.Instance; } }

        public static HashSet<Type> AutoSaveTypes = new HashSet<Type>();

        //ChangeWatcher ChangeWatcher;
        public HandleAutoSaveManager()
        {
            //ChangeWatcher = new ChangeWatcher();
        }

        public void Register(IHandleBase handle)
        {
            throw new NotImplementedException("TODO: H<TValue>.ObjectChanged");
#if TODO
            // TODO MEMORYLEAK ToWeakEvents
            handle.ObjectChanged += handle_ObjectChanged;
            //ChangeWatcher.Add(handle.Object, handle);
            l.Trace("[autosave] Registered " + handle.ToStringSafe()); // TEMP
#endif
        }

        void handle_ObjectChanged(IHandleBase handle, string propertyName)
        {
#if TODO
            var isValid = handle.Object as IIsValid;

            // Abort saving if reports !IsValid, and handle has yet to be persisted
            if (isValid != null && !isValid.IsValid)
            {
                if (!handle.IsPersisted())
                {
                    l.Debug("[autosave] UNTESTED - Not autosaving changes because this object is not persisted and it is not valid yet: " + handle.ToStringSafe());
                    return;
                }
            }

            ThrottledSaveManager.Instance.OnChanged(handle);
#endif
        }

        public void Unregister(IHandleBase handle)
        {
            throw new NotImplementedException("TODO: H<TValue>.ObjectChanged");
#if TODO
            handle.ObjectChanged -= handle_ObjectChanged;
            l.Trace("[autosave] Unregistered " + handle.ToStringSafe()); // TEMP
#endif
        }

        #region Misc

        private static readonly ILogger l = Log.Get();

        #endregion
    }

}
