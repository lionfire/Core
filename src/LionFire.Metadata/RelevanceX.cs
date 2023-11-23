using LionFire.Metadata;
using System.Reflection;

namespace LionFire.ExtensionMethods.Metadata;

public static class RelevanceX
{
    public static RelevanceFlags GetRelevanceFlags(this Type type, RelevanceAspect direction)
        => type.GetCustomAttributes<RelevanceAttribute>().GetRelevanceFlags(direction);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="memberInfo"></param>
    /// <param name="direction"></param>
    /// <param name="fallbackType">If no RelevanceAttributes are found on MemberInfo, fall back to this type.  If null, it will use memberInfo.DeclaringType.  If you wish to skip this, use typeof(object)</param>
    /// <returns></returns>
    public static RelevanceFlags GetRelevanceFlags(this MemberInfo memberInfo, RelevanceAspect direction, Type? fallbackType = null)
    {

        var result = memberInfo.GetCustomAttributes<RelevanceAttribute>().GetRelevanceFlags(direction);
        if (result == RelevanceFlags.Unspecified)
        {
            fallbackType ??= memberInfo.DeclaringType;
            if (fallbackType != null && fallbackType != typeof(object))
            {
                result = fallbackType.GetRelevanceFlags(direction);
            }
        }
        return result;
    }

    public static RelevanceFlags GetRelevanceFlags(this IEnumerable<RelevanceAttribute> attributes, RelevanceAspect aspect)
    {
        RelevanceFlags result = RelevanceFlags.Unspecified;
        foreach (var attr in attributes)
        {
            if (attr.Aspect.HasFlag(aspect))
            {
                result |= attr.Relevance;
            }
        }
        return result;
    }
}