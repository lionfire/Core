using System;

namespace LionFire.MultiTyping
{
    public interface INotifyMultiTypeChanged
    {

        void AddTypeHandler(Type type, Action<IMultiTyped, Type> callback);
        void RemoveTypeHandler(Type type, Action<IMultiTyped, Type> callback);
        //void AddTypeHandler<T>(Type type, MulticastDelegate callback) where T:class;
        //void RemoveTypeHandler<T>(Type type, MulticastDelegate callback) where T:class;
    }
}
