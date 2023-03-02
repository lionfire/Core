using System;

namespace LionFire.FlexObjects
{
    public static class FlexGlobalOptions
    {
        public static Func<Type, object> DefaultCreateFactory { get; set; } = type => Activator.CreateInstance(type);
        public static Func<Type, object[], object> DefaultCreateWithOptionsFactory { get; set; } = (type,args) => Activator.CreateInstance(type, args);
    }

}
