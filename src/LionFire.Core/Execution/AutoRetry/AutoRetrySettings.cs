using LionFire.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution;

// TODO: Use this.  Maybe have a key/value system: AutoRetry("Filesystem") AutoRetry("Web")
public class AutoRetrySettings 
{
    public int DefaultMaxRetries = 50;
    public bool ThrowLastExceptionOnFail = true;
    public int DefaultMillisecondsBetweenAttempts = 200;

    #region Static

    static AutoRetrySettings() {
        defaultSettings = new AutoRetrySettings()
        {
        };
    }

    public static AutoRetrySettings Default => defaultSettings;
    static readonly AutoRetrySettings defaultSettings;
    
    #endregion

}
