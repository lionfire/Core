//using LionFire.Inspection;
//using System.Reflection;

//namespace LionFire.Inspection;

///// <summary>
///// For C# Properties that are accessible via Index (GetIndexParameters().Length > 0)
///// </summary>
//public class IndexPropertyVM : DataReflectionMember<PropertyInfo, ReflectionIndexPropertyInfo>
//{
//    public IndexPropertyVM(ReflectionMemberInfo info, object source) : base(info, source)
//    {
//    }

//    public override PropertyInfo DataMemberInfo => ReflectionMemberInfo.PropertyInfo;
//    //public override ReflectionIndexPropertyInfo ReflectionMemberInfo => (ReflectionIndexPropertyInfo)Info;
//    //public override object? GetValue() => throw new NotSupportedException();
//    //public override bool CanGetValue => false;
//    //public override Type DataType => DataMemberInfo.PropertyType;
//}
