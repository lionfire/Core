using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Validation
{
    public class ValidationContext // RENAME to ValidationResult?
    {
        public ValidationContext() { }
        public ValidationContext(object obj, object validationKind=null) { this.Object = obj;  ValidationKind = validationKind; }

        public object ValidationKind { get; set; }
        public List<ValidationIssue> Issues { get; private set; }
        public object Object { get; set; }

        public bool IsValid => Issues == null || Issues.Count == 0;
        public bool HasIssues => !IsValid;

        public void AddIssue(ValidationIssue issue)
        {
            if (Issues == null) { Issues = new List<ValidationIssue>(); }
            Issues.Add(issue);
        }
    }

}
