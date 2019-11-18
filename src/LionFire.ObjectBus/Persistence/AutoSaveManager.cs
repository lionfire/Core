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
    // MOVE this to LionFire.Persistence.Handles and rename to HandleAutoSaveManager?

    public class AutoSaveManager
    {
        public static AutoSaveManager Instance { get { return Singleton<AutoSaveManager>.Instance; } }

        public static HashSet<Type> AutoSaveTypes = new HashSet<Type>();

        //ChangeWatcher ChangeWatcher;
        public AutoSaveManager()
        {
            //ChangeWatcher = new ChangeWatcher();
        }

        public void Register(IReadWriteHandle<object> handle)
        {
            throw new NotImplementedException("TODO: figure out the change interfaces on handles");
#if TODO
            // TODO MEMORYLEAK ToWeakEvents
            handle.ValueChanged += OnHandleObjectChanged;
            //ChangeWatcher.Add(handle.Object, handle);
#if TRACE_Autosave
            l.LogTrace("[autosave] Registered " + handle?.ToString()); 
#endif
#endif
        }

        void OnHandleObjectChanged(IReadHandleBase<object> handle)
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

            ThrottledSaveManager.Instance.OnChanged((IReadWriteHandleBase<object>)handle);
        }

        public void Unregister(IReadWriteHandleBase<object> handle)
        {
            throw new NotImplementedException("TODO: figure out the change interfaces on handles");
#if TODO
            handle.ObjectChanged -= OnHandleObjectChanged;
#if TRACE_Autosave
            l.LogTrace("[autosave] Unregistered " + handle?.ToString()); 
#endif
#endif
        }

        #region Misc

        private static ILogger l = Log.Get();

        #endregion
    }

    public static class AutoSaveManagerExtensions
    {
        public static void SetAutosave(this IReadWriteHandle<object> obj, bool enabled = true)
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
