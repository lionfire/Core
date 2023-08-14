using LionFire.Mvvm;
using System.Reflection;

namespace LionFire.Inspection;

// For Fields or (indexed or non-indexed) Properties
public abstract class DataReflectionMember<TMemberInfo, TReflectionMemberInfo> : ReflectionMemberVM, IReflectionDataMemberVM
    where TMemberInfo : System.Reflection.MemberInfo
    where TReflectionMemberInfo : ReflectionMemberInfo
{
    #region Identity    

    public abstract TMemberInfo DataMemberInfo { get; }
    System.Reflection.MemberInfo IReflectionDataMemberVM.MemberInfo  => (System.Reflection.MemberInfo)DataMemberInfo;

    #endregion

    #region Lifecycle

    protected DataReflectionMember(ReflectionMemberInfo info, object source) : base(info, source)
    {
    }

    #endregion

    //public abstract TReflectionMemberInfo ReflectionMemberInfo { get; }
    public TReflectionMemberInfo ReflectionMemberInfo => (TReflectionMemberInfo)Info;

    #region State

    // REVIEW - may be AsyncValue or AsyncGets?
    //[Reactive]
    //public AsyncVM<object> Value { get; set; }

    #endregion


    //public Exception? GetException { get; set; }
    //public abstract object? GetValue();
    //public object Refresh() { GetException = null; return GetValue(); }

    //public virtual bool CanGetValue
    //{
    //    get
    //    {
    //        if (!Info.CanRead()) return false;
    //        if (!IsSupportedType) return false;
    //        return true;
    //    }
    //}

    //public virtual bool IsIndexed => DataMemberInfo is PropertyInfo pi && pi.GetIndexParameters().Length > 0;


    public virtual bool IsSupportedType
    {
        get
        {
            var type = Info.Type;
            var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            if (genericType != null)
            {
                if (genericType == typeof(ReadOnlySpan<>)) { return false; }
            }
            return true;
        }
    }
    //public abstract Type DataType { get; }

}
