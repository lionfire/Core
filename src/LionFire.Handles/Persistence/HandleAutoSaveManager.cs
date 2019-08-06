using LionFire.Persistence;
using LionFire.Structures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using LionFire.Referencing;

namespace LionFire.ObjectBus
{
    public static class HandleAutoSaveManagerExtensions
    {
        public static void SetAutosave(this IHandle obj, bool enabled = true)
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

        public void Register(IHandle handle)
        {
            // TODO MEMORYLEAK ToWeakEvents
            handle.ObjectChanged += handle_ObjectChanged;
            //ChangeWatcher.Add(handle.Object, handle);
            l.Trace("[autosave] Registered " + handle.ToStringSafe()); // TEMP
        }

        void handle_ObjectChanged(IHandle handle, string propertyName)
        {
            IIsValid isValid = handle.Object as IIsValid;

            // Abort saving if reports !IsValid, and handle has yet to be persisted
            if (isValid != null && !isValid.IsValid)
            {
                if (!handle.IsPersisted)
                {
                    l.Debug("[autosave] UNTESTED - Not autosaving changes because this object is not persisted and it is not valid yet: " + handle.ToStringSafe());
                    return;
                }
            }

            ThrottledSaveManager.Instance.OnChanged(handle);
            
        }

        public void Unregister(IHandle handle)
        {
            handle.ObjectChanged -= handle_ObjectChanged;
            l.Trace("[autosave] Unregistered " + handle.ToStringSafe()); // TEMP
        }

        #region Misc
        
        private static ILogger l = Log.Get();
		
        #endregion
    }

}
