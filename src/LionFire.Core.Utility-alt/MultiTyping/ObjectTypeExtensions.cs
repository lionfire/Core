// OLD - replaced with ObjectAsType
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using LionFire.MultiTyping;
//using LionFire.Types;

//namespace LionFire.ObjectBus
//{
//    public static class ObjectTypeExtensions
//    {
//        /// <summary>
//        /// Try to upcast to IMultiType and use its AsType, otherwise fall back to as operator
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="obj"></param>
//        /// <returns></returns>
        
//#if !AOT
//		public static T AsType<T>(object obj)
//            where T : class
//        {
//            // Try Upcasting
//            IReadonlyMultiTyped mt = obj as IReadonlyMultiTyped;
//            if (mt != null)
//            {
//                return mt.AsType<T>();
//            }

//            return obj as T;
//        }
//#endif
//    }
//}
