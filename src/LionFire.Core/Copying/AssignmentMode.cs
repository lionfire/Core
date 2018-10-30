
namespace LionFire.Copying
{
    // TODO: LionFire.Copyable
    // Mode: Ignore/Assign/Deep, Flags: ICloneable

    /// <summary>
    /// RENAME to copy mode.  TODO Have CopyFrom/AssignFrom/DeepCopyFrom methods that pass this as an arg
    /// </summary>
    public enum AssignmentMode 
    {
        Unspecified = 0,

        /// <summary>
        /// Do not touch the target member
        /// </summary>
        Ignore,

        /// <summary>
        /// Do a straight reference/value assignment 
        /// RENAME - shallow copy?
        /// </summary>
        Assign,

        /// <summary>
        /// If child implements IClonable, clone it.  Otherwise, Assign
        /// REVIEW - make this a flag that modifies the other options?
        /// </summary>
        CloneIfCloneable,

        /// <summary>
        /// RENAME Deep copy
        /// </summary>
        DeepCopy,
    }
}
