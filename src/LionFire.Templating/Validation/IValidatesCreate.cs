using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Validation;

namespace LionFire.Templating
{
    public interface IValidatesCreate
    {
        ValidationContext ValidateCreate(ValidationContext context);
    }
    public static class ValidatesCreateExtensions
    {
        public static ValidationContext ValidateCreate(this ValidationContext ctx)
        {
            ((IValidatesCreate)ctx.Object).ValidateCreate(ctx);
            return ctx;
        }
    }
}
