using LionFire.Execution;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Validation
{
    public class ValidationIssue
    {

        public ValidationIssue() { }
        public ValidationIssue(string message) { this.Message = message; }
        public ValidationIssue(object data)
        {
            if (data is string s) { Message = s; }
            else Data = data;
        }

        public static implicit operator ValidationIssue(string msg)
        {
            return new ValidationIssue(msg);
        }

        public string Key { get; set; }

        // REVIEW - don't have hardcoded enums here
        public ValidationIssueKind Kind { get; set; }

        /// <summary>
        /// Set for PropertyNotSet, FieldNotSet, MemberNotSet, ArgumentNotSet
        /// </summary>
        public string VariableName { get; set; }

        public object Data { get; set; }

        public string Message
        {
            get { return message ?? Data?.ToString(); }
            set { message = value; }
        }
        private string message;

        public string Detail { get; set; }
        //public Dictionary<string, object> Properties { get; set; }

        public IEnumerable<ValidationIssue> InnerIssues { get; set; }

    }
}
