﻿using LionFire.Mvvm;
using LionFire.UI.Metadata;
using System.Reflection;

namespace LionFire.UI.Components.PropertyGrid;

// For Fields or Properties
public abstract class DataMemberVM<TMemberInfo, TMemberInfoVM> : MemberVM, IDataMemberVM
    where TMemberInfo : MemberInfo
{
    public override MemberKind MemberKind => MemberKind.Data;

    public abstract TMemberInfo DataMemberInfo { get; }
    MemberInfo IDataMemberVM.MemberInfo  => (MemberInfo)DataMemberInfo;

    public abstract TMemberInfoVM DataMemberInfoVM { get; }

    [Reactive]
    public AsyncVM<object> Value { get; set; }

    public Exception? GetException { get; set; }
    public abstract object? GetValue();
    public object Refresh() { GetException = null; return GetValue(); }

    public virtual bool CanGetValue
    {
        get
        {
            if (!MemberInfoVM.CanRead) return false;
            if (!IsSupportedType) return false;
            return true;
        }
    }
    public virtual bool IsIndexed => DataMemberInfo is PropertyInfo pi && pi.GetIndexParameters().Length > 0;

    
    public virtual bool IsSupportedType
    {
        get
        {
            var type = MemberInfoVM.Type;
            var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            if (genericType != null)
            {
                if (genericType == typeof(ReadOnlySpan<>)) { return false; }
            }
            return true;
        }
    }
    public abstract Type DataType { get; }

}
