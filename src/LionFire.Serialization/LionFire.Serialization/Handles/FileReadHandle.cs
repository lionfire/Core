using LionFire.DependencyInjection;
using LionFire.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Handles
{

    public class FileReadHandle<T> : IReadHandle<T>
    {
        public T Object
        {
            get
            {
                var ser = InjectionContext.Current.GetService<IFileDeserializer>();
                if (ser == null) throw new HasUnresolvedDependenciesException($"No {typeof(IFileDeserializer).Name} available");
            }
        }

        public bool HasObject => throw new NotImplementedException();

        string Path { get; }
    }
}
