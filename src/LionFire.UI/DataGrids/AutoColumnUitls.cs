using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI;

public class AutoColumnUtils
{
    public static bool DefaultIsAutoColumn(PropertyInfo propertyInfo)
    {
        var typeName = propertyInfo.PropertyType.FullName!;

        if (typeName.StartsWith("System.IObservable`1")
            || typeName.StartsWith("DynamicData.SourceCache")
            || typeName.StartsWith("DynamicData.SourceList")
            ) return false;

        var attr1 = GetPropertyOrUnderlyingFieldCustomAttribute<BrowsableAttribute>(propertyInfo);
        if (attr1 != null && !attr1.Browsable) return false;
        var attr2 = GetPropertyOrUnderlyingFieldCustomAttribute<RelevanceAttribute>(propertyInfo);
        if (attr2 != null && !attr2.Relevance.HasFlag(RelevanceFlags.User))
        {
            if (attr2.Relevance.HasFlag(RelevanceFlags.Opaque)
                || attr2.Relevance.HasFlag(RelevanceFlags.Internal)
                ) return false;
        }

        //var attr = propertyInfo.GetCustomAttribute<BrowsableAttribute>();
        //if (attr != null && !attr.Browsable) return false;

        //FieldInfo? fieldInfo = propertyInfo.DeclaringType!.GetField($"_{char.ToLower(propertyInfo.Name[0]) + propertyInfo.Name.Substring(1)}", BindingFlags.NonPublic | BindingFlags.Instance)
        // ?? propertyInfo.DeclaringType.GetField($"{char.ToLower(propertyInfo.Name[0]) + propertyInfo.Name.Substring(1)}", BindingFlags.NonPublic | BindingFlags.Instance);

        //if (fieldInfo != null)
        //{
        //    var browsableAttr = fieldInfo.GetCustomAttribute<BrowsableAttribute>();
        //    if (browsableAttr != null && !browsableAttr.Browsable) return false;
        //}

        return true;
    }

    public static T? GetPropertyOrUnderlyingFieldCustomAttribute<T>(PropertyInfo propertyInfo) where T : Attribute
    {
        return propertyInfo.GetCustomAttribute<T>() ?? GetUnderlyingFieldCustomAttribute<T>(propertyInfo);
    }
    public static T GetUnderlyingFieldCustomAttribute<T>(PropertyInfo propertyInfo) where T : Attribute
    {
        FieldInfo? fieldInfo = propertyInfo.DeclaringType!.GetField($"_{char.ToLower(propertyInfo.Name[0]) + propertyInfo.Name.Substring(1)}", BindingFlags.NonPublic | BindingFlags.Instance)
         ?? propertyInfo.DeclaringType.GetField($"{char.ToLower(propertyInfo.Name[0]) + propertyInfo.Name.Substring(1)}", BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo != null)
        {
            var attr = fieldInfo.GetCustomAttribute<T>();
            if (attr != null) return attr;
        }
        return null!;
    }
}
