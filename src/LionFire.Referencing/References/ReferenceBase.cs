using LionFire.Referencing.Resolution;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public class ReferenceBase : IReference
    {
        public  IHandleResolver HandleResolver => throw new NotImplementedException();

        public string Uri => throw new NotImplementedException();

        public string Scheme => throw new NotImplementedException();

        public string Host => throw new NotImplementedException();

        public string Port => throw new NotImplementedException();

        public string Path => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string Package => throw new NotImplementedException();

        public string Location => throw new NotImplementedException();

        public string TypeName => throw new NotImplementedException();

        public Type Type => throw new NotImplementedException();

        public bool IsLocalhost => throw new NotImplementedException();

        public string Key => throw new NotImplementedException();

        public IReference GetChild(string subPath)
        {
            throw new NotImplementedException();
        }

        public IReference GetChildSubpath(IEnumerable<string> subpath)
        {
            throw new NotImplementedException();
        }

        public IHandle<T> GetHandle<T>(T obj = null) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
