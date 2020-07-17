using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Diagnostics
{
    public static class LogProcess
    {
        public static Process LogOutput(this Process proc, bool stdOut = true, bool stdErr = true, string logName = null)
        {
            if (proc.StartInfo == null)
            {
                proc.StartInfo = new ProcessStartInfo();
            }
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.StartInfo.RedirectStandardOutput = stdOut;
            proc.StartInfo.RedirectStandardError = stdErr;

            ILogger logger;
            if (logName != null) { logger = Log.Get(logName); }
            else { logger = l; }

            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {

                if (stdErr)
                {
                    proc.ErrorDataReceived += (s, e) =>
                    {
                        try
                        {
                            if (e.Data == null)
                            {
                                errorWaitHandle.Set();
                            }
                            else
                            {
                                logger.Warn(e.Data);
                            }
                        }
                        catch (ObjectDisposedException)
                        {
                            //l.Warn("Got ObjectDisposedException");
                        }
                    };
                }
                if (stdOut)
                {
                    proc.OutputDataReceived += (s, e) =>
                    {
                        try
                        {
                            if (e.Data == null)
                            {
                                outputWaitHandle.Set();
                            }
                            else
                            {
                                if (e.Data.ToLowerInvariant().Contains("error "))
                                {
                                    logger.Error(e.Data);
                                }
                                else if (e.Data.ToLowerInvariant().Contains("warning"))
                                {
                                    logger.Warn(e.Data);
                                }
                                else {
                                    logger.Info(e.Data);
                                }
                            }
                        }
                        catch (ObjectDisposedException)
                        {
                         //   l.Warn("Got ObjectDisposedException");
                        }
                    };
                }
            }

            return proc;
        }
        public static Process LogOutputAndStart(this Process proc, bool stdOut = true, bool stdErr = true, string logName = null)
        {
            proc.LogOutput(stdOut, stdErr, logName);
            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            return proc;
        }

        private static ILogger l = Log.Get();

    }
}
