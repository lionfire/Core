//using AppUpdate;
//using AppUpdate.Common;

namespace LionFire.Shell
{
    public class UpdaterShell
    {
#if TOPORT

        bool deferredAskUserToUpdate;

    #region AutoUpdate

        /// <summary>
        /// MODAL Updates are available, so ask the user if they wish to download and install them  now.
        /// </summary>
        /// <returns>false if application should stop what it's doing and update itself</returns>
        public bool AskUserToUpdate()
        {
            if (!IsStarted)
            {
                deferredAskUserToUpdate = true; // Defer until WPF Application is started
                                                // FUTURE: could defer only if we need to ask the user something
                return true;
            }
            deferredAskUserToUpdate = false; // reset

            l.Info("[AUTOUPDATER] UPDATE AVAILABLE");

            // TODO: Hide the UpdateManager reference in the Application framework?
            var dr = MessageBox.Show(
                    string.Format("Updates are available to your software ({0} total). Do you want to download and prepare them now? You can always do this at a later time.",
                        updaterShell.UpdatesAvailable
                    //UpdateManager.Instance.UpdatesAvailable
                    ),
                    "Software updates available",
                     MessageBoxButton.YesNo);

            if (dr == MessageBoxResult.Yes)
            {
                return false; // Signifies the app should stop initializing and go forward with the update!
            }
            return true;
        }

        //private void OnAutoUpdateCheckFinished(bool needsUpdate)
        //{
        //    if (needsUpdate)
        //    {                
        //        AskUserToUpdate();
        //    }
        //    else
        //    {
        //        l.Info("App is up to date");
        //    }
        //}
    #endregion
#endif

    }

}
