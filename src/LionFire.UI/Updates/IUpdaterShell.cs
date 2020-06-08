namespace LionFire.Shell
{
    // TODO: REVIEW and Refactor this to be built more around Generic Host

    public interface IUpdaterShell
    {
        bool UpdatesAvailable { get; }

        // TODO: Reevaluate this

        //  - Checkbox: always autodownload
        //  - Checkbox: always autoupdate
        //  - Notice of where to show, if "do not show again"
        //  - Update after close
        //  - Update now

        /// <summary>
        /// If returns false, suspend App initialization.  Shell must call LionFireApp.ResumeInit()
        /// </summary>
        /// <returns>false if the updater needs to restart.  In this case, the app initialization
        /// should cease.</returns>
        bool AskUserToUpdate();
    }
}
