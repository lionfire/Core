#if TOPORT
using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Alerting;
using Microsoft.Extensions.Logging;

namespace LionFire.Shell;

// TOPORT - moved this out of ShellContentPresenter.  Not sure it is used.  Move to some sort of "vos/persistence document management framework" in LionFire.UI.Persistence?

public class DocumentManager
{
#if NEW
    Use H<object>
#endif

#if TOPORT
    public IHandle CurrentDocumentHandle
    {
        get
        {
            var fe = CurrentDocumentTab as FrameworkElement;
            if (fe == null) return null;

            var handle = fe.DataContext as IHandle;
            if (handle != null) return handle;

            var hasHandle = fe.DataContext as IHasHandle;
            if (hasHandle != null) return hasHandle.Handle;

            return null;
        }
    }
#endif

    public bool Save(FailAction failAction = FailAction.AlertUser)
    {
#if TOPORT
        var doc = this.CurrentDocumentHandle;

        try
        {
            if (doc == null)
            {
                //throw new Exception("No document found in CurrentDocumentHandle");
                return true;
            }

#if NEW
            doc.Commit();
#else
            doc.Save();
#endif
            l.Info("Saved: " + doc.Reference);
            return true;
        }
        catch (Exception ex)
        {
            l.Warn("Save exception:  " + ex);
            if (failAction == FailAction.AlertUser)
            {
                Alerter.Alert("Failed to save", ex);
            }
            else if (failAction == FailAction.ThrowException)
            {
                throw;
            }
            return false;
        }
#else
        throw new NotImplementedException();
#endif
    }

    private static ILogger l = Log.Get();


}
#endif