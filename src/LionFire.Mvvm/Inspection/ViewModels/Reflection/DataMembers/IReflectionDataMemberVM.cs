using LionFire.Mvvm;
using System.Reflection;

namespace LionFire.Inspection;

public interface IDataMemberVMBase
{

}

public interface IReflectionDataMemberVM
{
    //object GetValue();
    //bool CanGetValue { get; }

    System.Reflection.MemberInfo MemberInfo { get; }

    //Type DataType { get; }

    //AsyncVM<object> Value { get; set; }
}
