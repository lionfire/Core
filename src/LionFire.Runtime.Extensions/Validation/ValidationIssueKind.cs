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

        Other = 1 << 30,
    }
}
