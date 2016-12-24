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
        public static ValidationContext Validate(this object obj)
        {
            return new ValidationContext() { Object = obj };
        }
        public static ValidationContext MemberNonNull(this ValidationContext ctx, object val, string memberName)
        {
            if (val == null)
            {
                ctx.AddIssue(new ValidationIssue
                {
                    MemberName = memberName,
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
                    MemberName = pi.Name,
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
                    MemberName = propertyName,
                });
            }
            return ctx;
        }
    }
}

