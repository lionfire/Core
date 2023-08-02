using LionFire.Mvvm;
using System.Reflection;

namespace LionFire.Mvvm.ObjectInspection;

public interface IDataMemberVMBase
{

}

public interface IDataMemberVM
{
    //object GetValue();
    //bool CanGetValue { get; }

    System.Reflection.MemberInfo MemberInfo { get; }

    //Type DataType { get; }

    //AsyncVM<object> Value { get; set; }
}
