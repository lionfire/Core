using LionFire.DependencyInjection;
using LionFire.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Handles
{

    public class FileReadHandle<T> : IReadHandle<T>
        where  T: class
    {
        public T Object
        {
            get
            {
                if (obj == null)
                {
                    TryResolveObject();
                }
                return obj;
            }
        }
        private T obj;

        public bool HasObject => throw new NotImplementedException();

        string Path { get; }

        public void ForgetObject() { obj = null; }
        public void TryResolveObject()
        {
            var ser = InjectionContext.Current.GetService<ISerializationService>();
            if (ser == null) throw new HasUnresolvedDependenciesException($"No {typeof(ISerializationService).Name} available");
            this.obj = ser.FileToObject<T>(Path);
        }
    }
}
