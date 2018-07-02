using System;
using System.Linq;
using System.Reflection;
using LionFire.Extensions;

namespace LionFire.Validation
{
    public static class CommonValidateExtensions
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

        public static ValidationContext PropertyNotNull(this ValidationContext ctx, string propertyName, object propertyValue = null)
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
        public static ValidationContext PropertyNotSet(this ValidationContext ctx, PropertyInfo pi, object obj = null)
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
        public static ValidationContext PropertyNonDefault<T>(this ValidationContext ctx, string propertyName, T value)
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

        public static void ValidateMethods(this ValidationContext ctx, object obj = null)
        {
            if (obj == null) { obj = ctx.Object; }
            if (obj == null) { throw new ArgumentNullException("obj == null and ctx.Object == null"); }

            // OPTIMIZE: Use Fody to add a method "ValidateMethods" to the obj and invoke that instead of reflecting over method attributes

            foreach (var mi in obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.GetCustomAttribute<ValidateAttribute>() != null))
            {
                mi.Invoke(obj, new object[] { ctx });
            }
        }

    }
}

