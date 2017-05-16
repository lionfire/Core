using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Validation
{
    public enum ValidationIssueKind
    {
        Unspecified = 0,
        MissingDependency = 1 << 0,
        PropertyNotSet = 1 << 1,
        FieldNotSet = 1 << 2,
        InvalidConfiguration = 1 << 3,
        //InvalidState = 1 << 4,
        ParameterOutOfRange= 1 << 5,
        ArgumentNotSet = 1 << 6,
        /// <summary>
        /// More vague version of Property/Field Not Set
        /// </summary>
        MemberNotSet = 1 << 7,

        Other = 1 << 30,
    }
}
