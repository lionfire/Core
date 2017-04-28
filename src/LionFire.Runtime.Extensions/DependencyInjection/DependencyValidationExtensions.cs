using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LionFire.Validation
{
    public static class DependencyValidationExtensions
    {
        public static  ValidationContext DependenciesAreSet(this ValidationContext validationContext)
        {
            if (validationContext.Object == null) throw new ArgumentNullException("validationContext.Object");

            foreach (var pi in validationContext.Object.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (pi.GetCustomAttribute<DependencyAttribute>() == null) continue;
                if (!pi.PropertyType.IsByRef) continue;

                if (pi.GetValue(validationContext.Object) == null)
                {
                    validationContext.AddIssue(new ValidationIssue
                    {
                        Message = $"Missing dependency '{pi.Name}'"
                    });
                }
            }
            return validationContext;
        }
    }
}
