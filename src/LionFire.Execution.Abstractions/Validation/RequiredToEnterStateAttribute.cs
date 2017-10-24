using LionFire.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public sealed class RequiredToEnterStateAttribute : Attribute
    {
        public ExecutionStateEx State { get; private set; }
        public RequiredToEnterStateAttribute(ExecutionStateEx state)
        {
            this.State = state;
        }
    }

    public static class ExecutionValidateExtensions
    {
        public static ValidationContext CanEnterState(this ValidationContext ctx, ExecutionStateEx state)
        {
            foreach (var pi in ctx.Object.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var attrs = pi.GetCustomAttributes<RequiredToEnterStateAttribute>();
                if (attrs.Where(a => a.State == state).Any())
                {
                    ctx.PropertyNonDefault(pi);
                }
            }
            return ctx;
        }
    }
}