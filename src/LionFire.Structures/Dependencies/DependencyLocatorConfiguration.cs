namespace LionFire.Dependencies
{
    public static class DependencyLocatorConfiguration
    {
        /// <summary>
        /// Indicates whether the current process should prefer singletons where appropriate.
        /// If true, ManualSingleton.GuaranteedInstance is the recommended implementation.  
        /// If false, another mechanism such as DependencyContext.Current.GetService() should be preferred.
        /// 
        /// The default is true, for performance and simplicity in the typical case.
        /// </summary>
        public static bool UseSingletons { get; set; } = true;

        public static bool UseDependencyContext { get; set; } = true;


        public static bool AllowDefaultSingletonActivatorOnDemand { get; set; } = true;


        public static bool UseIServiceProviderSingleton { get; set; } = false;
    }
}
