using LionFire.UI.Metadata;
using System.Reflection;

namespace LionFire.UI.Components.PropertyGrid;

public class PropertyVM : DataMemberVM<PropertyInfo, PropertyInfoVM>
{
    public override PropertyInfo DataMemberInfo => DataMemberInfoVM.PropertyInfo;
    public override PropertyInfoVM DataMemberInfoVM => (PropertyInfoVM)MemberInfoVM;
    public override object? GetValue()
    {
        if (GetException != null) return GetException;
        try
        {
            return DataMemberInfo.GetValue(Target);
        }
        catch(Exception ex)
        {
            GetException = ex;
            return ex;
        }
    }
    public override Type DataType => DataMemberInfo.PropertyType;
}
