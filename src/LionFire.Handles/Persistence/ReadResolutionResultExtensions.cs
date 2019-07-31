//using LionFire.Referencing;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace LionFire.Referencing.Persistence
//{
//    public static class ResolutionResultExtensions
//    {
//        public static RH<T> GetReadHandle<T>(this ReadResolutionResult<T> rrr)
//        {
//            if (rrr.ReadHandle == null && rrr.Reference != null)
//            {
//                rrr.ReadHandle = rrr.Reference.GetReadHandle<T>();
//            }
//            return rrr.ReadHandle;
//        }

//        public static H<T> GetHandle<T>(this WriteResolutionResult<T> wrr)
//        {
//            if (wrr.Handle == null && wrr.Reference != null)
//            {
//                wrr.Handle = wrr.Reference.GetHandle<T>();
//            }
//            return wrr.Handle;
//        }
//    }
//}
