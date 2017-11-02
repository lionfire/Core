using LionFire.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.Extensions;
using LionFire.Execution;

namespace LionFire.Validation
{
    public static class ValidateExtensions
    {


        /// <summary>
        /// Repeatedly cycle through a group of objects, running validateAction on them until all validate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validatables">List of objects to validate</param>
        /// <param name="validateAction">Some sort of intitialization/validation action to perform</param>
        /// <param name="maxRepetitions">Max repetitions to perform on a failure.  Set to zero to only attempt validation once.</param>
        /// <returns>Null on success, or a list of failing ValidationContexts on fail</returns>
        public static async Task<IEnumerable<ValidationContext>> TryValidateAll<T>(this IEnumerable<T> validatables, Func<T, Task<ValidationContext>> validateAction, int maxRepetitions = int.MaxValue)
        {
            int lastFailCount = int.MaxValue;

            List<ValidationContext> fails = new List<ValidationContext>();

            do
            {
                foreach (var validatable in validatables)
                {
                    var result = await validateAction(validatable).ConfigureAwait(false);
                    if (result == null || result.Valid) continue;
                    if (fails == null) fails = new List<ValidationContext>();
                    fails.Add(result);
                }
                if (fails != null)
                {
                    lastFailCount = fails.Count;
                    fails.Clear();
                }
            } while (fails.Count > 0 && fails.Count < lastFailCount && maxRepetitions-- > 0);
            return fails.Count == 0 ? null : fails;
        }

        /// <summary>
        /// Repeatedly cycle through a group of objects, running validateAction on them until all return null.  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects">List of objects to validate</param>
        /// <param name="action">Some sort of intitialization/validation action to perform</param>
        /// <param name="maxRepetitions">Max repetitions to perform on a failure.  Set to zero to only attempt validation once.</param>
        /// <returns>Null on success, or a list of failing ValidationContexts on fail</returns>
        public static async Task<IEnumerable<object>> TryResolveAll<T>(this IEnumerable<T> objects, Func<T, Task<object>> action, int maxRepetitions = int.MaxValue)
        {
            int lastFailCount = int.MaxValue;

            List<object> fails = null;

            do
            {
                if (fails != null)
                {
                    lastFailCount = fails.Count;
                }
                fails = new List<object>();

                foreach (var validatable in objects)
                {
                    var result = await action(validatable).ConfigureAwait(false);
                    if (result == null) continue;
                    fails.Add(result);
                }
            } while (fails.Count > 0 && fails.Count < lastFailCount && maxRepetitions-- > 0);
            return fails.Count == 0 ? null : fails;
        }

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

        /// <summary>
        /// If context is null, it is treated as valid
        /// </summary>
        /// <param name="context"></param>
        public static void EnsureValid(this ValidationContext context)
        {
            if (context != null && !context.Valid)
            {
                throw new ValidationException(context);
            }
        }
        public static bool IsValid(this ValidationContext context)
        {
            return context == null || context.Valid;
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

