using System.Reflection;
using System.Runtime.Serialization;

namespace LionFire.Inspection;

public class PropertyVM : DataReflectionMember<PropertyInfo, ReflectionPropertyInfo>
{
    public PropertyVM(ReflectionMemberInfo info, object source) : base(info, source)
    {
    }

    public override PropertyInfo DataMemberInfo => ReflectionMemberInfo.PropertyInfo;
    //public override ReflectionPropertyInfo ReflectionMemberInfo => (ReflectionPropertyInfo)Info;
    //public override object? GetValue()
    //{
    //    if (GetException != null) return GetException;
    //    try
    //    {
    //        return DataMemberInfo.GetValue(Source);
    //    }
    //    catch(Exception ex)
    //    {
    //        GetException = ex;
    //        return ex;
    //    }
    //}
    //public override Type DataType => DataMemberInfo.PropertyType;
}
