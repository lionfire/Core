using Microsoft.Extensions.Logging;

namespace LionFire.Vos
{
    public class DefaultVosContextResolver : IVosContextResolver
    {
        public Vob GetVobRoot(string path, VosContext context)
        {
            return GetVobRoot(path, context.Package, context.Store);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Leave null for root path</param>
        /// <param name="package"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public Vob GetVobRoot(string path= null, string package=null, string location=null)
        {
            Vob vob;

            if (package == null)
            {
                if (location != null)
                {
                    //l.Trace("GetVobRoot from Context: location specified but no package.");
                    vob = VosApp.Instance.Stores[location][path];
                }
                else // All null
                {
                    vob = VosContext.DefaultRoot[path];
                }

            }
            else // package != null
            {
                if (location == null)
                {
                    vob = VosApp.Instance.Packages[package][path];
                }
                else
                {
                    vob = VosApp.Instance.PackageStores[package][location][path];
                }
            }
            return vob;
        }

        private static ILogger l = Log.Get();

    }
}

