//using LionFire.ExtensionMethods;
//using LionFire.Referencing;
//using System;
//using System.Linq;
//using System.Text;

//namespace LionFire.Referencing
//{
//#if !AOT
//    /// <summary>
//    /// Caches Reference URIs to IHandles
//    /// </summary>
//    /// <typeparam name="T">Type of Object</typeparam>
//    public static class HandleFactory<T> // RENAME to HandleTracker or HandleCache
//        where T : class//, new()
//    {

//        public static H<T> CreateHandle(IReference reference, T obj = null)
//            //where T:class
//        {
//            H<T> handle = new HDynamic<T>(reference, obj);
//            return handle;
//        }

//        [Obsolete("Moved to HandleTracker")]
//        public static H<T> ToHandle(IReference reference, T obj = null)
//        //where T:class
//        {
//            return HandleProvider<T>.ToHandle(reference, obj);
//        }         

//        public static H<T> CreateHandle(IReference reference)
//        {
//            var handle = new HDynamic<T>(reference);
//            return handle;
//        }
//    }
//#endif

//    public static class HandleFactory
//    {
//        [Obsolete("Moved to HandleProvider")]
//        public static H ToHandle(IReference reference, object obj = null) => (H)HandleProvider<object>.ToHandle(reference, obj);
//        public static H CreateHandle(IReference reference) => (H)HandleFactory<object>.CreateHandle(reference);
//    }


//}
