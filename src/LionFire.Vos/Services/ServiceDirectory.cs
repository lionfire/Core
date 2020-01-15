namespace LionFire.Vos.Services
{

    /// <summary>
    /// REVIEW - new idea, minimalist implementation
    /// </summary>
    public class ServiceDirectory
    {
        public Vob Root { get; }

        public ServiceDirectory(RootVob root)
        {
            Root = root;
        }

        public static string ServiceDirectoryRoot = $"/_/services/";

        public static string ServiceDirectoryLocation<TService>(string name = null, string serviceName = null)
        {
            serviceName = serviceName ?? typeof(TService).Name;
            var path = ServiceDirectoryRoot + serviceName;
            if (name != null) path += "/" + name;
            return path;
        }
        public IVob ServiceDirectoryVob<TService>(string name = null, string serviceName = null, bool createIfMissing = true)
            => Root.Root.GetOrQueryChild(ServiceDirectoryLocation<TService>(name, serviceName), createIfMissing: createIfMissing);

        public void RegisterService<TService, TValue>(TValue value, string name = null, string serviceName = null)
            => ServiceDirectoryVob<TService>(name, serviceName).Value = value;

        public TValue FindServiceEntry<TService, TValue>(string name = null, string serviceName = null) where TValue : class
            => ServiceDirectoryVob<TService>(name, serviceName, createIfMissing: false)?.Value as TValue;

        public TService FindService<TService>(string name = null, string serviceName = null)
            where TService : class
        {
            var path = ServiceDirectoryVob<TService>(name, serviceName, createIfMissing: false)?.Value as string;
            if (path == null) return default;
            var vob = Root[path];

            var service = vob.GetOwn<TService>();
            if (service != default) return service;

            service = vob.GetMultiTyped().AsType<TService>();

            return service;
        }
    }
}
