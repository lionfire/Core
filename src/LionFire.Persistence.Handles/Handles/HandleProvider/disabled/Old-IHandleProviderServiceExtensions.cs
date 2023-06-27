//using System;
//using LionFire.Dependencies;

//namespace LionFire.Referencing
//{
//    public static class IHandleProviderServiceExtensions
//    {
//        private static IHandleProviderService CurrentHandleProviderService => DependencyContext.Current.GetService<IHandleProviderService>();

//        //public static H<TValue> ToHandle<TValue>(this string uri) => new UriStringReference(uri).ToHandle<TValue>();
//        //public static H<TValue> ToHandle<TValue>(this Uri uri) => new UriReference(uri).ToHandle<TValue>();
//        public static H<TValue> ToHandle<TValue>(this string uri) where TValue : class => CurrentHandleProviderService.ToHandle<TValue>(new UriStringReference(uri));
//        public static H<TValue> ToHandle<TValue>(this Uri uri) where TValue : class => CurrentHandleProviderService.ToHandle<TValue>(new UriReference(uri));
//        public static H<TValue> ToHandle<TValue>(this IReference reference) where TValue : class => CurrentHandleProviderService.ToHandle<TValue>(reference);

//        public static H<TValue> ObjectToHandle<TValue>(this Uri uri) where TValue : class => CurrentHandleProviderService.ToHandle<TValue>(new UriReference(uri));

//        //// TODO: Add more obj params
//        //public static H<object> ToHandle(this string uri) // Add obj?
//        //{
//        //    return HandleProvider.ToHandle(uri.ToReference());
//        //}

//    }
    
//}
