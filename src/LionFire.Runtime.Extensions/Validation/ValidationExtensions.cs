using LionFire.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.Extensions;

namespace LionFire.Validation
{
    public static class ValidateExtensions
    {
        public static ValidationContext Validate(this object obj, object validationKind = null)
        {
            return new ValidationContext() { Object = obj, ValidationKind = validationKind ?? ValidationKind.Unspecified };
        }

        public static ValidationContext IsTrue(this ValidationContext ctx, bool func, Func<ValidationIssue> issue)
        {
            if (!func) ctx.AddIssue(issue());
            return ctx;
        }
        public static ValidationContext IsTrue(this ValidationContext ctx, bool func, ValidationIssue issue)
        {
            if (!func) ctx.AddIssue(issue);
            return ctx;
        }

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

        public static void EnsureValid(this ValidationContext ctx)
        {
            if (!ctx.IsValid)
            {
                throw new ValidationException(ctx);
            }
        }

        public static ValidationContext PropertyNonDefault(this ValidationContext ctx, PropertyInfo pi, object obj = null)
        {
            if ((obj ?? ctx.Object).IsDefault(pi))
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
            if (value.IsDefault())
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

