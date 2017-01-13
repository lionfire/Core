using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Validation
{
    public class ValidationContext
    {
        public object ValidationKind { get; set; }
        public List<ValidationIssue> Issues { get; private set; }
        public bool IsValid { get { return Issues == null || Issues.Count == 0; } }
        public object Object { get; set; }

        public void AddIssue(ValidationIssue issue)
        {
            if (Issues == null) { Issues = new List<ValidationIssue>(); }
            Issues.Add(issue);
        }
    }

}
