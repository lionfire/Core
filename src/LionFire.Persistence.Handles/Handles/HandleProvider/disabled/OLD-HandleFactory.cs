// OLD?
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
//    /// <typeparam name="TValue">Type of Object</typeparam>
//    public static class HandleFactory<TValue> // RENAME to HandleTracker or HandleCache
//        where TValue : class//, new()
//    {

//        //public static H<TValue> CreateHandle(IReference reference, TValue obj = null)
//        //    //where TValue:class
//        //{
//        //    H<TValue> handle = new HDynamic<TValue>(reference, obj);
//        //    return handle;
//        //}

//        [Obsolete("Moved to HandleTracker")]
//        public static H<TValue> ToHandle(IReference reference, TValue obj = null)
//        //where TValue:class
//        {
//            return HandleProvider<TValue>.ToHandle(reference, obj);
//        }         

//        //public static H<TValue> CreateHandle(IReference reference)
//        //{
//        //    var handle = new HDynamic<TValue>(reference);
//        //    return handle;
//        //}
//    }
//#endif

//    //public static class HandleFactory
//    //{
//    //    [Obsolete("Moved to HandleProvider")]
//    //    public static H ToHandle(IReference reference, object obj = null) => (H)HandleProvider<object>.ToHandle(reference, obj);
//    //    public static H CreateHandle(IReference reference) => (H)HandleFactory<object>.CreateHandle(reference);
//    //}


//}
