#define TRACE_Autosave
using LionFire.ObjectBus;
using LionFire.Persistence;
using LionFire.Reflection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Structures;

namespace LionFire.ObjectBus
{

    public class AutoSaveManager
    {
        public static AutoSaveManager Instance { get { return Singleton<AutoSaveManager>.Instance; } }

        public static HashSet<Type> AutoSaveTypes = new HashSet<Type>();

        //ChangeWatcher ChangeWatcher;
        public AutoSaveManager()
        {
            //ChangeWatcher = new ChangeWatcher();
        }

        public void Register(IReadHandle<object> handle)
        {
            // TODO MEMORYLEAK ToWeakEvents
            handle.ObjectChanged += handle_ObjectChanged;
            //ChangeWatcher.Add(handle.Object, handle);
#if TRACE_Autosave
            l.LogTrace("[autosave] Registered " + handle?.ToString()); 
#endif
        }

        void handle_ObjectChanged(IReadHandle<object> handle, object oldObject, object newObject)
        {
            IIsValid isValid = handle.Object as IIsValid;

            // Abort saving if reports !IsValid, and handle has yet to be persisted
            if (isValid != null && !isValid.IsValid) // TODO: Use something like Validate(PurposeKind.Persistence), to save partially valid works in progress
            {
                if (!handle.IsPersisted)
                {
                    l.Debug("[autosave] UNTESTED - Not autosaving changes because this object is not persisted and it is not valid yet: " + handle.ToStringSafe());
                    return;
                }
            }

            ThrottledSaveManager.Instance.OnChanged(handle);
            
        }

        public void Unregister(IReadHandle<object> handle)
        {
            handle.ObjectChanged -= handle_ObjectChanged;
#if TRACE_Autosave
            l.LogTrace("[autosave] Unregistered " + handle?.ToString()); 
#endif
        }

        #region Misc

        private static ILogger l = Log.Get();
		
        #endregion
    }

    public static class AutoSaveManagerExtensions
    {
        public static void SetAutosave(this IHandle obj, bool enabled = true)
        {
            if (enabled)
            {
                AutoSaveManager.Instance.Register(obj);
            }
            else
            {
                AutoSaveManager.Instance.Unregister(obj);
            }

        }
    }
}
