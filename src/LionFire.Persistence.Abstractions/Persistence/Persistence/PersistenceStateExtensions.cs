namespace LionFire.Persistence.Handles
{
    public static class PersistenceStateExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">The state to modify</param>
        /// <param name="flags">A flag or set of flags to set or unset.</param>
        /// <param name="isSet">If true (default), set the flags.  If false, unset the flags.</param>
        public static void SetFlag(this ref PersistenceFlags state, PersistenceFlags flags, bool isSet = true)
        {
            if (isSet) { state |= state; }
            else { state &= ~flags; }
        }
        public static void UnsetFlag(this ref PersistenceFlags state, PersistenceFlags flags) => state.SetFlag(flags, false);
    }
}
