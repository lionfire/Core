﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using LionFire.Dependencies;

//namespace LionFire.Referencing
//{
//    public class HandleProviderService : IHandleProviderService
//    {
//        public IEnumerable<Type> HandleTypes => DependencyContext.Current.GetService<IEnumerable<IReadWriteHandleProvider>>().SelectMany(hp => hp.HandleTypes);

//        public H<TValue> ToHandleOrThrow<TValue>(IReference reference) where TValue : class
//        {
//            var result = ToHandleOrThrow<TValue>(reference);
//            if (result != null) return result;

//            throw new Exception($"Failed to provide handle for reference with scheme {reference.Scheme}.  Have you registered the relevant IReadWriteHandleProvider service?");
//        }

//        public H<TValue> ToHandle<TValue>(IReference reference, TValue handleObject = default(TValue))
//        {
//            foreach(var hp in DependencyContext.Current.GetService<IEnumerable<IReadWriteHandleProvider>>())
//            {
//                var h = hp.ToHandle<TValue>(reference, handleObject);
//                if (h != null) return h;
//            }

//            return null;
//        }
//    }
//}

