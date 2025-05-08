using LionFire.Extensions;
using System.Reflection;

namespace LionFire.Validation;

public static class ValidateDefaultValueX
{
    public static ValidationContext PropertyNotDefault(this ValidationContext ctx, PropertyInfo pi, object? obj = null)
    {
        if ((obj ?? ctx.Object).IsDefaultValue(pi))
        {
            ctx.AddIssue(new ValidationIssue
            {
                Kind = ValidationIssueKind.PropertyNotSet,
                VariableName = pi.Name,
            });
        }
        return ctx;
    }
    public static ValidationContext PropertyNotDefault<T>(this ValidationContext ctx, string propertyName, T value)
    {
        if (value.IsDefaultValue())
        {
            ctx.AddIssue(new ValidationIssue
            {
                Kind = ValidationIssueKind.PropertyNotSet,
                VariableName = propertyName,
            });
        }
        return ctx;
    }
}

