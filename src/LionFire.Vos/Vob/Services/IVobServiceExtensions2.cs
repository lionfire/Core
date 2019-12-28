//using System;
//using LionFire.Dependencies;

//namespace LionFire.Vos.ExtensionMethods
//{
//    public static class IVobServiceExtensions2
//    {
//        public static T GetService<T>(this IVob vob) => (T)((IVobNode<IServiceProvider>)vob.GetNext<IServiceProvider>())?.GetService(typeof(T));

//        public static T GetService<T>(this Vob vob)
//        {
//            return (T)vob.GetNext<IServiceProvider>()?.GetService(typeof(T));
//        }
//        public static T GetRequiredService<T>(this Vob vob)
//        {
//            var serviceProvider = vob.GetNextRequired<IServiceProvider>();
//            var result = (T)serviceProvider.GetService(typeof(T));
//            if (result == null) throw new HasUnresolvedDependenciesException($"{typeof(T).FullName} not available in Vob ancestry services");
//            return result;
//        }
//    }
//}
