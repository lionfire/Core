using LionFire.Data.Gets;
using LionFire.Data.Mvvm;
using LionFire.Data.Reactive;
using LionFire.Data.Sets;
using LionFire.IO;
using System.Reflection;

namespace LionFire.Mvvm.ObjectInspection;


//public class FuncMemberInfo<T> : CustomMemberInfo
//{

//    public Func<object, T> Func { get; }

//    public FuncMemberInfo(string name, Type type, Func<object, T> func) : base(name, type)
//    {
//        Func = func;
//    }

//    public GetsMemberVM<object> Create(object obj)
//    {
//        return Func(obj);
//    }
//}

//public class GetsMemberInfo : FuncMemberInfo<ILazilyGetsRx<object>>
//{
//    public override IODirection IODirection => IODirection.Read;

//    public GetsMemberInfo(string name, Type type, Func<object, ILazilyGetsRx<object>> func) : base(name, type, func)
//    {
//    }
//}

public class GetsMemberVM<T> : MemberVM<IInspectorMemberInfo, IGetsRx<T>>
{
    public GetsMemberVM(IInspectorMemberInfo info, IGetsRx<T> gets) : base(info, gets)
    {
        //Gets = gets;
    }

    //public ILazilyGetsRx<T> Gets { get; init; }

}

// TODO:

//public override IODirection IODirection => IODirection.Read;
//public class SetsMemberVM<T> : MemberVM<SetsMemberInfo>
//{
//    public ISetsRx<T> Sets { get; init; }
//}




//public interface IAsyncGetsMemberVM<T> : ILazilyGetsVM<T>
//{

//}

//public class AsyncGetsMemberVM<T> : MemberVM
//{
//    IAsyncGetsRx
//}


// OLD: 

//public class AsyncPropertyVM : MemberVM
//{
//    public object Source { get; init; }

//    public AsyncPropertyInfo AsyncPropertyInfoVM { get; set; }

//    public override PropertyInfo DataMemberInfo => throw new NotImplementedException();

//    public override PropertyInfoVM DataMemberInfoVM => throw new NotImplementedException();

//    //ILazilyGetsRx<T>
//    ////, ISetsRx<T>

//    public override Type DataType => AsyncPropertyInfoVM.Type;

//    public override MemberKind MemberKind => throw new NotImplementedException();

//    public override object? GetValue()
//    {
//        throw new NotImplementedException();
//    }
//}