using LionFire.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                    var validationContext = await validateAction(validatable).ConfigureAwait(false);
                    if (validationContext.Valid) continue;
                    if (fails == null) fails = new List<ValidationContext>();
                    fails.Add(validationContext);
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
            return new ValidationContext() { Object = obj, ValidationKind = validationKind };
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
    }
}

