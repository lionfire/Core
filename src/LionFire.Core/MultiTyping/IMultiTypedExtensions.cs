﻿using LionFire.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.MultiTyping
{
    public static class IMultiTypedExtensions
    {
        public static void SetType<T>(this IMultiTyped mt, T obj) where T : class => mt[typeof(T)] = obj;

        public static IEnumerable<Type> SubTypeTypes(this IMultiTyped mt) => mt.SubTypes.Select(st => st.GetType());

        [ThreadSafe(false)]
        public static void AddType<T>(this IMultiTyped mt, T obj) where T : class
        {
            if (mt[typeof(T)] != null) throw new AlreadyException();
            mt.SetType<T>(obj);
        }
    }
}
