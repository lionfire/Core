using System;

namespace LionFire.Resolves.ChainResolving
{
    public class ChainResolverWorker
    {
        public Type FromType { get; set; }
        public Type ToType => Delegate?.Method.ReturnType;

        public Delegate Delegate { get; set; }

        #region Constrution

        public ChainResolverWorker(Type instanceType, Func<object, object> func)
        {
            FromType = instanceType;
            //Func = func;
            Delegate = func;
        }

        public ChainResolverWorker(Type instanceType, Delegate del)
        {
            if (del.Method.GetParameters().Length < 1) throw new ArgumentException($"{nameof(del)} must have at least one parameter.");

            FromType = instanceType;
            Delegate = del;
        }
        
        #endregion
    }
}
