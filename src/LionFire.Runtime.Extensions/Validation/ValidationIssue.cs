using LionFire.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Validation
{
    public class ValidationIssue
    {
        public ValidationIssueKind Kind { get; set; }

        /// <summary>
        /// Set for PropertyNotSet, FieldNotSet
        /// </summary>
        public string MemberName { get; set; }
       

        public string Message { get; set; }

        public string Detail { get; set; }
        //public Dictionary<string, object> Properties { get; set; }

        public IEnumerable<ValidationIssue> InnerIssues { get; set; }

    }
}
