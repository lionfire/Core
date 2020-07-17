#if TODO
using System;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public interface IVobEnvironment
    {
        string GetEnvironmentString(string key);
        IVobReference GetEnvironmentReference(string key);
        T GetEnvironment<T>(string key);
    }

    public class VobEnvironment2 : VobNodeBase<VobEnvironment>, IVobEnvironment
    {
        public string[] SubPath = new string[] { "_", "env" };

        public IVob InternalEnvironmentVob
        {
            get
            {
                if (internalEnvironmentVob == null)
                {
                    internalEnvironmentVob = Vob.GetChild(SubPath);
                }
                return internalEnvironmentVob;
            }
        }
        private IVob internalEnvironmentVob;

       
        public string GetEnvironmentString(string key)
        {
            var obj = Vob.QueryChild("_", "env", key)?.Value;
            return obj switch
            {
                null => null,
                string s => s,
                IReference r => r.Key,
                _ => throw new InvalidCastException($"Conversion from type {obj.GetType().FullName} to string not supported."),
            };
        }
        public IVobReference GetEnvironmentReference(string key)
        {
            var obj = Vob.QueryChild("_", "env", key)?.Value;
            return obj switch
            {
                null => null,
                IVobReference r => r,
                _ => throw new InvalidCastException($"Conversion from type {obj.GetType().FullName} to IVobReference not supported."),
            };
        }
        public T GetEnvironment<T>(string key)
        {
            var obj = Vob.QueryChild("_", "env", key)?.Value;
            return obj switch
            {
                null => null,
                T t => t,
                _ => throw new InvalidCastException($"Conversion from type {obj.GetType().FullName} to {typeof(T).FullName} not supported."),
            };
        }


    }

    public static class VobEnvironmentExtensions
    {
        public static IVobEnvironment Environment(this IVob vob)
        {

        }
    }
}

#endif