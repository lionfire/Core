namespace LionFire.DependencyMachines
{
    public enum ParticipantFlags
    {
        Unspecified = 0,

        // FUTURE:
        StopAfterInputsChanged = 1 << 1,
        StartAfterInputsChanged = 1 << 2,

        /// <summary>
        /// OBSOLETE - StartTask will return non-null if it is retryable.  (If there was a flag, it could be to disallow RoundRobin, in which case non-null return values would be thrown.)
        /// May fail.  If so, rerun all other remaining items at the current stage, then try again.  Only fail the entire process if no progress is made.
        /// FUTURE
        /// </summary>
        //RoundRobin = 1 << 1,
        ThrowOnError = 1 << 3,

        // ENH
        //AutoStopOnDisappearingDependencies = 1 << ,

        StageEnder = 1 << 4,

        // ENH: Option for exclusive ender/starter per stage. 


    }
}
