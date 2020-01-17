namespace LionFire.FlexObjects
{
    public class FlexMemberOptions
    {
        // When are options used?
        //  - Add - check to see if already exists.  If so, throw if AllowMultiple = false
        //  - GetOrAddDefault - use registered factory for type, or the default generic factory.
        public bool AllowMultiple { get; set; }
    }

}
