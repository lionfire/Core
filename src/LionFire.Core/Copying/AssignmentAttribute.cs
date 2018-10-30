using System;
using System.Collections.Generic;

namespace LionFire.Copying
{
    // TODO: LionFire.Copyable
    // Mode: Ignore/Assign/Deep, Flags: ICloneable

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class AssignmentAttribute : Attribute
    {
        public AssignmentAttribute(AssignmentMode assignmentMode)
        {
            this.assignmentMode = assignmentMode;
        }

        public AssignmentMode AssignmentMode
        {
            get { return assignmentMode; }
        }
        readonly AssignmentMode assignmentMode;
    }
}