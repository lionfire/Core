//using LionFire.Referencing;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace LionFire.Referencing.Persistence
//{
//    public static class ResolutionResultExtensions
//    {
//        public static RH<TValue> ToReadHandle<TValue>(this ReadResolutionResult<TValue> rrr)
//        {
//            if (rrr.ReadHandle == null && rrr.Reference != null)
//            {
//                rrr.ReadHandle = rrr.Reference.ToReadHandle<TValue>();
//            }
//            return rrr.ReadHandle;
//        }

//        public static H<TValue> ToHandle<TValue>(this WriteResolutionResult<TValue> wrr)
//        {
//            if (wrr.Handle == null && wrr.Reference != null)
//            {
//                wrr.Handle = wrr.Reference.ToHandle<TValue>();
//            }
//            return wrr.Handle;
//        }
//    }
//}
