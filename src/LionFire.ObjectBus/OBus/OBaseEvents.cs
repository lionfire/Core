//#define TRACE_GET
#define TRACE_GET_NOTFOUND

#if NET4
#define WEAKMETADATA // Experimental way to attach various info
#endif
//#define USE_READCACHE
using System;
using Microsoft.Extensions.Logging;
using LionFire.Referencing;
using LionFire.Persistence;

namespace LionFire.ObjectBus
{
    public static class OBaseEvents
    {
        public static bool ValidateOnRetrieve = true;

        /// <summary>
        /// Retrieved from a bottom-tier data source.
        /// </summary>
        /// <param name="obj"></param>
        public static void OnRetrievedObjectFromExternalSource(object obj)
        {
            if (ValidateOnRetrieve)
            {
                IValidatable v = obj as IValidatable;
                if (v != null)
                {
                    v.Validate();
                }
            }

            if (obj is INotifyOnRetrieve nor) nor.OnRetrieved();
        }

        public static void OnSaving(object obj)
        {
            if (obj is INotifyOnSaving nos) nos.OnSaving();

            if (obj is ILastModified lm) lm.LastModified = DateTime.Now;

        }

        public static void OnException(OBusOperations operation, IReference r, Exception ex)
        {
            l.Error("Exception during " + operation.ToString() + " for " + r + ": " + ex.ToStringSafe());
            throw ex;
        }
        private static ILogger l = Log.Get("LionFire.OBus");
    }
}
