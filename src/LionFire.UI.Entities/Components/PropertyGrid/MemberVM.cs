using LionFire.Mvvm;
using LionFire.UI.Metadata;
using ReactiveUI;
using System.Reflection;

namespace LionFire.UI.Components.PropertyGrid;

public abstract class MemberVM : ReactiveObject
{
    #region Dependencies

    public MemberInfoVM MemberInfoVM { get; set; }

    #endregion
}

// For Fields or Properties
public abstract class DataMemberVM<TMemberInfo> : MemberVM
    where TMemberInfo : MemberInfo
{
    [Reactive]
    public AsyncVM<object> Value { get; set; }
}

public class PropertyVM : DataMemberVM<PropertyInfo>
{
}

public class FieldVM : DataMemberVM<FieldInfo>
{
}

public class MethodVM : MemberVM
{

}