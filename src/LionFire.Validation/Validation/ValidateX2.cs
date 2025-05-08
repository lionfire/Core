using System;
using System.Linq;
using System.Reflection;

namespace LionFire.Validation;

// REVIEW: Merge with ValidateX, or find a better way to organize them
public static class ValidateX2
{ 

    public static ValidationContext MemberNonNull(this ValidationContext ctx, object val, string memberName)
    {
        if (val == null)
        {
            ctx.AddIssue(new ValidationIssue
            {
                VariableName = memberName,
                Kind = ValidationIssueKind.MemberNotSet,
            });
        }
        return ctx;
    }
    public static ValidationContext ArgumentNonNull(this ValidationContext ctx, object val, string memberName)
    {
        if (val == null)
        {
            ctx.AddIssue(new ValidationIssue
            {
                VariableName = memberName,
                Kind = ValidationIssueKind.ArgumentNotSet,
            });
        }
        return ctx;
    }

    /// <summary>
    /// If context is null, it is treated as valid
    /// </summary>
    /// <param name="context"></param>
    public static void EnsureValid(this ValidationContext context)
    {
        if (!context.Valid)
        {
            throw new ValidationException(context);
        }
    }
    public static bool IsValid(this ValidationContext context)
    {
        return context.Valid;
    }

#nullable enable
    public static ValidationContext PropertyNotNull(this ValidationContext ctx, string propertyName, object? propertyValue = null)
    {
        if (propertyValue == null)
        {
            ctx.AddIssue(new ValidationIssue
            {
                Kind = ValidationIssueKind.PropertyNotSet,
                VariableName = propertyName,
            });
        }
        return ctx;
    }
    public static ValidationContext PropertyNotNull(this ValidationContext ctx, PropertyInfo propertyInfo)
    {
        var propertyValue = propertyInfo.GetValue(ctx.Object);
        return ctx.PropertyNotNull(propertyInfo.Name, propertyValue);
    }

    

    public static void ValidateMethods(this ValidationContext ctx, object? obj = null)
    {
        if (obj == null) { obj = ctx.Object; }
        if (obj == null) { throw new ArgumentNullException("obj == null and ctx.Object == null"); }

        // OPTIMIZE: Use SourceGenerators to add a method "ValidateMethods" to the obj and invoke that instead of reflecting over method attributes

        foreach (var mi in obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.GetCustomAttribute<ValidateAttribute>() != null))
        {
            mi.Invoke(obj, new object[] { ctx });
        }
    }

}

