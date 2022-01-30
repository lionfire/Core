using LionFire.Dependencies;
using LionFire.FlexObjects;

namespace LionFire.Vos.Services
{
    /// <summary>
    /// A way to register services of arbitrary type in a Vos tree.  
    /// Primary keys: service type name (uses Name from type if unspecified), 
    /// and optional name (empty if unspecified.)
    /// There can only be one service registered per type+name combo.
    /// </summary>
    /// <remarks>
    /// Services by convention are registered at StatePath = /_/services/<serviceType>[/serviceInstanceName]
    ///  - Service object referenced are attached via IVob.Value
    /// </remarks>
    public class ServiceDirectory
    {
        #region Conventions

        public static string StatePath = $"/_/services/";

        #endregion

        #region Construction

        public ServiceDirectory(RootVob root)
        {
            ServiceDirectoryRoot = root;
            State = ServiceDirectoryRoot[StatePath];
        }

        #endregion

        #region State

        public Vob ServiceDirectoryRoot { get; }
        public IVob State { get; }

        #endregion

        #region (Static)

        public static string ServiceStateRelativePath<TService>(string name = null, string serviceType = null)
        {
            serviceType ??= typeof(TService).Name;
            var path = serviceType;
            if (name != null) path += "/" + name;
            return path;
        }

        public static TService GetServiceFromStateVob<TService>(IVob vob)
            where TService : class 
            => vob?.FlexData as TService;

        #endregion

        #region (Public)

        public void Register<TService>(TService value, string name = null, string serviceType = null)
            => ServiceStateVob<TService>(name, serviceType).Add(value);

        public TService GetService<TService>(string name = null, string serviceType = null) where TService : class 
            => ServiceStateVob<TService>(name, serviceType, createIfMissing: false).FlexData as TService;

        //public TService FindService<TService>(string name = null, string serviceType = null)
        //    where TService : class
        //{
        //    var path = ServiceStateVob<TService>(name, serviceType, createIfMissing: false)?.Value as string;
        //    if (path == null) return default;
        //    var vob = Root[path];
        //    var service = vob.AcquireOwn<TService>();
        //    if (service != default) return service;
        //    service = vob.GetMultiTyped().AsType<TService>();
        //    return service;
        //}

        public TService GetRequiredService<TService>(string name = null, string serviceType = null) 
            where TService : class
            => GetService<TService>(name, serviceType) 
            ?? throw new DependencyMissingException($"Service not registered: type = '{serviceType}', name = '{name}'");

        #endregion

        #region (Protected)

        protected IVob ServiceStateVob<TService>(string name = null, string serviceType = null, bool createIfMissing = true)
            => ServiceDirectoryRoot.GetOrQueryChild(ServiceStateRelativePath<TService>(name, serviceType), createIfMissing: createIfMissing);

        #endregion
    }
}
