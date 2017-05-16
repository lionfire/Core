using LionFire.Applications.Hosting;
using LionFire.Composables;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace LionFire.Serialization.Json.Newtonsoft
{
    public class AppPackage : IAdding
    {

        #region MOVE to IConfiguresServices

        public virtual IEnumerable<ServiceDescriptor> ServiceDescriptors { get { yield break; } }

        public virtual void ConfigureServices(IServiceCollection sc)
        {
        }

        #endregion

        #region MOVE to IAddsComponents


        public virtual IEnumerable<object> Components { get { yield break; } }

        #endregion

        #region MOVE to [NotAddedAsComponent]

        public virtual bool AddPackageAsComponent => false;

        #endregion

        #region This will be deprecated after above

        public virtual bool OnAdding<T>(IComposable<T> parent)
        {
            var app = parent as IAppHost;
            if (app != null)
            {
                foreach (var sd in ServiceDescriptors) app.ServiceCollection.Add(sd);
            }

            foreach (var sd in Components) app.Add(sd);

            return AddPackageAsComponent;
        }

        #endregion

    }
}
