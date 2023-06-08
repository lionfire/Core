using LionFire.Mvvm;
using System.Reflection;

namespace LionFire.UI.Components.PropertyGrid;

public interface IDataMemberVM
{
    object GetValue();
    bool CanGetValue { get; }

    MemberInfo MemberInfo { get; }

    Type DataType { get; }

    AsyncVM<object> Value { get; set; }
}
