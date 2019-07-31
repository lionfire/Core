namespace LionFire.Applications
{
    public static class SingletonConfiguration
    {
        /// <summary>
        /// Indicates whether the current process should prefer singletons where appropriate.
        /// If true, ManualSingleton.Guaranteed is the recommended implementation.  
        /// If false, another mechanism such as DependencyContext.Current.GetService() should be preferred.
        /// 
        /// The default is true, for performance and simplicity in the typical case.
        /// </summary>
        public static bool UseSingletons { get; set; } = true;
    }
}
