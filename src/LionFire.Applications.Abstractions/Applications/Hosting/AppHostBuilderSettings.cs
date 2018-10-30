namespace LionFire.Applications.Hosting
{
    public static class AppHostBuilderSettings
    {
        /// <summary>
        /// If possible, set ManualSingleton&lt;TService&gt;.Instance to the provided implementation, or to the eventual result of the provided factory.
        /// </summary>
        public static bool SetManualSingletons { get; set; } = true;
    }
}
