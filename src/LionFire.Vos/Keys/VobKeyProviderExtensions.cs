using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public static class VobKeyProviderExtensions // MOVE to vob namespace?
    {
        public static bool WarnOnNewKeyExists = true;
        public static int MaxGenerateKeyAttempts = 10;

        // FUTURE: 

        // Optimizing ID contention policy:
        //  - new items store the sync authority, app, machine, then network block, then network

        // Create conflict idea:  attach a GUID to each conflicting member,

        // Create conflict policies:
        //  - If referenceable:
        //    - 
        //  - If nonreferenceable:
        //    - assign new Ids to both
        //    - assign new Ids to both

        public static object GenerateKeyFromServer(this Vob vob)
        {
            throw new NotImplementedException();
        }

        public static object GenerateKeyFromReservedBlock(this Vob vob)
        {
            throw new NotImplementedException();
        }

        public static object GenerateNonReferencableKey(this Vob vob)
        {
            throw new NotImplementedException();
        }

        public static object GenerateAutoIncrementedKey(this Vob vob)
        {
            throw new NotImplementedException();
        }

        public static object GenerateGuidKey(this Vob vob)
        {
            throw new NotImplementedException();
        }

        public static string GenerateStringKey(this Vob vob)
        {
            var kp = VobKeyProvider.GetOrCreateDefault(vob);
            return kp.Object.GetNextKeyAsString();
        }

        // FUTURE:  Atomic file move/creation?
        // Not sure if this would help: http://blogs.msdn.com/b/adioltean/archive/2005/12/28/507866.aspx

        public static VobHandle<T> ConstructChild<T>(this Vob vob)
            where T : class
        {
            VobHandle<T> vh = null;
            for(int i = MaxGenerateKeyAttempts; i > 0; i--)
            {
                var key = GenerateStringKey(vob) as string;

                 vh = new VobHandle<T>(vob,  key);

                if (vh.Exists)
                {
                    if (WarnOnNewKeyExists)
                    {
                        l.Warn("Generated key already exists: " + vh.Key);
                    }
                    vh=null;
                }
                else
                {
                    break;
                }
            }

            if (vh == null) throw new VosException("Failed to construct child.  Try increasing VobKeyProviderExtensions.MaxGenerateKeyAttempts");

            return vh;
        }
        public static VobHandle<T> CreateChild<T>(this Vob vob, T child = null)
            where T : class
        {
            var vh = ConstructChild<T>(vob);

            if (child == null)
            {
                child = Activator.CreateInstance<T>();
            }
            vh.Object = child;
            vh.Save();

            return vh;
        }

        #region Misc

        private static ILogger l = Log.Get();

        #endregion
    }
}
