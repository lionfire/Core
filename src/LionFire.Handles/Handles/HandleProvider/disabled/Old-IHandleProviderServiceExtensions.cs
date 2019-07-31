//using System;
//using LionFire.DependencyInjection;

//namespace LionFire.Referencing
//{
//    public static class IHandleProviderServiceExtensions
//    {
//        private static IHandleProviderService CurrentHandleProviderService => DependencyContext.Current.GetService<IHandleProviderService>();

//        //public static H<T> ToHandle<T>(this string uri) => new UriStringReference(uri).ToHandle<T>();
//        //public static H<T> ToHandle<T>(this Uri uri) => new UriReference(uri).ToHandle<T>();
//        public static H<T> ToHandle<T>(this string uri) where T : class => CurrentHandleProviderService.GetHandle<T>(new UriStringReference(uri));
//        public static H<T> ToHandle<T>(this Uri uri) where T : class => CurrentHandleProviderService.GetHandle<T>(new UriReference(uri));
//        public static H<T> ToHandle<T>(this IReference reference) where T : class => CurrentHandleProviderService.GetHandle<T>(reference);

//        public static H<T> ObjectToHandle<T>(this Uri uri) where T : class => CurrentHandleProviderService.GetHandle<T>(new UriReference(uri));

//        //// TODO: Add more obj params
//        //public static H<object> ToHandle(this string uri) // Add obj?
//        //{
//        //    return HandleProvider.GetHandle(uri.ToReference());
//        //}

//    }
    
//}
