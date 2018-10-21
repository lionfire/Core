using System.Collections.Generic;
using LionFire.DependencyInjection;

namespace LionFire.Referencing
{
    public class HandleProviderService : IHandleProviderService
    {
        public H<T> ToHandle<T>(IReference reference, bool throwOnFail) where T : class
        {
            foreach(var hp in InjectionContext.Current.GetService<IEnumerable<IHandleProvider>>())
            {
                var h = hp.ToHandle<T>(reference, false);
                if (h != null) return h;
            }

            if (throwOnFail) throw new System.Exception($"Failed to provide handle for reference with scheme {reference.Scheme}.  Have you registered the relevant IHandleProvider service?");

            return null;
        }
    }
}

