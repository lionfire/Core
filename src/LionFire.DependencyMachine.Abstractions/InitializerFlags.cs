namespace LionFire.DependencyMachine
{
    public enum InitializerFlags
    {
        Unspecified = 0,

        // FUTURE:
        RerunOnChangedInputs = 1 << 0,

        /// <summary>
        /// May fail.  If so, rerun all other remaining items at the current stage, then try again.  Only fail the entire process if no progress is made.
        /// FUTURE
        /// </summary>
        RoundRobin = 1 << 1,

        // ENH
        //AutoStopOnDisappearingDependencies = 1 << 2,
    }
}
