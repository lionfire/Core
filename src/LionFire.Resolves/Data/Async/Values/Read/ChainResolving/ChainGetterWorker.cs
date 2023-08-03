using System;

namespace LionFire.Data.Async.Gets.ChainResolving;

public class ChainGetterWorker
{
    public Type FromType { get; set; }
    public Type ToType => Delegate?.Method.ReturnType;

    public Delegate Delegate { get; set; }

    #region Constrution

    public ChainGetterWorker(Type instanceType, Func<object, object> func)
    {
        FromType = instanceType;
        //Func = func;
        Delegate = func;
    }

    public ChainGetterWorker(Type instanceType, Delegate del)
    {
        if (del.Method.GetParameters().Length < 1) throw new ArgumentException($"{nameof(del)} must have at least one parameter.");

        FromType = instanceType;
        Delegate = del;
    }
    
    #endregion
}
