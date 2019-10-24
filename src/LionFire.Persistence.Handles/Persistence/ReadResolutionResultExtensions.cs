//using LionFire.Referencing;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace LionFire.Referencing.Persistence
//{
//    public static class ResolutionResultExtensions
//    {
//        public static RH<T> ToReadHandle<T>(this ReadResolutionResult<T> rrr)
//        {
//            if (rrr.ReadHandle == null && rrr.Reference != null)
//            {
//                rrr.ReadHandle = rrr.Reference.ToReadHandle<T>();
//            }
//            return rrr.ReadHandle;
//        }

//        public static H<T> ToHandle<T>(this WriteResolutionResult<T> wrr)
//        {
//            if (wrr.Handle == null && wrr.Reference != null)
//            {
//                wrr.Handle = wrr.Reference.ToHandle<T>();
//            }
//            return wrr.Handle;
//        }
//    }
//}
