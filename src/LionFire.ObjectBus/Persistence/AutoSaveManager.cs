#define TRACE_Autosave
using LionFire.ObjectBus;
using LionFire.Persistence;
//using LionFire.Reflection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Structures;
using LionFire.Referencing;

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

        public void Register(W<object> handle)
        {
            // TODO MEMORYLEAK ToWeakEvents
            handle.ObjectChanged += OnHandleObjectChanged;
            //ChangeWatcher.Add(handle.Object, handle);
#if TRACE_Autosave
            l.LogTrace("[autosave] Registered " + handle?.ToString()); 
#endif
        }

        void OnHandleObjectChanged(RH<object> handle)
        {

            // Abort saving if reports !IsValid, and handle has yet to be persisted
            if (handle.Value is IIsValid isValid && !isValid.IsValid) // TODO: Use something like Validate(PurposeKind.Persistence), to save partially valid works in progress
            {
                if (!handle.IsPersisted())
                {
                    l.Debug("[autosave] UNTESTED - Not autosaving changes because this object is not persisted and it is not valid yet: " + handle.ToStringSafe());
                    return;
                }
            }

            ThrottledSaveManager.Instance.OnChanged((W<object>)handle);            
        }

        public void Unregister(W<object> handle)
        {
            handle.ObjectChanged -= OnHandleObjectChanged;
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
        public static void SetAutosave(this W<object> obj, bool enabled = true)
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
