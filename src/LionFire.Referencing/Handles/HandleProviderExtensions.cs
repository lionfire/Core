using LionFire.DependencyInjection;

namespace LionFire.Referencing
{
    public static class HandleProviderExtensions
    {
        public static H<T> ToHandle<T>(this string uriString) where T : class => InjectionContext.Current.GetService<IHandleProvider>().ToHandle<T>(new UriStringReference(uriString));
    }
}
