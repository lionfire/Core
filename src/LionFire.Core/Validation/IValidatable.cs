using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Validation
{

    public interface IValidatable
    {
        ValidationContext Validate(ValidationContext validationContext);
    }


    // ENH Ideas:
    // Validate(ExecutionState.Starting)
    // Looks on class for [RequiredForState(ExecutionState.Starting)]
    //  -- also traverses properties on child object [ChildrenRequiredForState(ExecutionState.Starting)]
}
