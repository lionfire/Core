using LionFire.Validation;
using System.Linq;
using System.Reflection;

namespace LionFire.Execution
{
    public static class ExecutionValidateExtensions
    {
        public static ValidationContext CanEnterState(this ValidationContext ctx, ExecutionStateEx state)
        {
            foreach (var pi in ctx.Object.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var attrs = pi.GetCustomAttributes<RequiredToEnterStateAttribute>();
                if (attrs.Where(a => a.State == state).Any())
                {
                    ctx.PropertyNotNull(pi);
                }
            }
            return ctx;
        }
    }
}