using System;

namespace LionFire.Resolves.ChainResolving
{
    public class ChainResolverWorker
    {
        public Type InstanceType { get; set; }
        //public Type[] ParameterTypes 

        //public Func<object, object> Func { get; set; }
        public Delegate Delegate { get; set; }


        public ChainResolverWorker(Type instanceType, Func<object, object> func)
        {
            InstanceType = instanceType;
            //Func = func;
            Delegate = func;
        }

        public ChainResolverWorker(Type instanceType, Delegate del)
        {
            if (del.Method.GetParameters().Length < 1) throw new ArgumentException($"{nameof(del)} must have at least one parameter.");

            InstanceType = instanceType;
            Delegate = del;
        }
    }
}
