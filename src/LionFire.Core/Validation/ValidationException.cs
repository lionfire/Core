using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Validation
{


    public class ValidationException : Exception
    {
        public ValidationContext Context { get; private set; }

        public object Reason
        {
            get { return reason; }
            set
            {
                reason = value;
                Context = new ValidationContext();
                Context.AddIssue(new ValidationIssue(value));
            }
        }
        private object reason;
        public ValidationException(ValidationContext ctx) { this.Context = ctx; }
        public ValidationException(object reason) { this.Reason = reason; }

        

        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }

        public override string ToString()
        {
            return nameof(ValidationException) + ":"+Environment.NewLine + (Context?.FailReasons()) + Environment.NewLine + StackTrace;
        }
    }
}
