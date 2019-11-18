using LionFire.Dependencies;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
#if NET35
using ManualResetEventSlim = System.Threading.ManualResetEvent; // REVIEW
#endif
using System.Threading.Tasks;
using LionFire.Shell;

namespace LionFire.Alerting
{
    public static class Alerter
    {
        public static bool ShowExceptionMessage = true;
        public static bool ShowExceptionDetail = true;

        public static IAlerter Instance => InstanceStackRegistrar.Default.TryResolve<IAlerter>();

        public static void Alert(Alert alert)
        {
            switch (alert.LogLevel)
            {
                // OLD - remove old log levels, add another log level property if desired
                //case LogLevel.Fatal:
                case LogLevel.Critical:
                    l.LogCritical(alert.Message);
                    break;
                case LogLevel.Error:
                    l.Error(alert.Message);
                    break;
                case LogLevel.Warning:
                    //case LogLevel.Warn:
                    l.Warn(alert.Message);
                    break;
                //case LogLevel.MajorMessage:
                //case LogLevel.Message:
                //case LogLevel.MinorMessage:
                case LogLevel.Information:
                    //case LogLevel.Default:
                    //def:  // REVIEW - Why can't mono handle this?
                    l.Info(alert.Message);
                    break;
                //case LogLevel.Verbose:
                case LogLevel.Debug:
                    l.Debug(alert.Message);
                    break;
                case LogLevel.Trace:
                    //case LogLevel.Step:
                    //case LogLevel.All:
                    l.Trace(alert.Message);
                    break;
                default:
                    l.Trace("TODO: use numeric values instead of switch.");
                    l.Info(alert.Message);
                    //goto def; // REVIEW - why does this not work in MonoDevelop on Mac?
                    break;
            }

            var instance = Instance;
            if (instance == null) { l.LogCritical("No IAlerter registered.  User alert not displayed: " + alert.Message); return; }

            instance.Alert(alert);
        }

        public static void Alert(string title, Exception ex)
        {
            Alert(ShowExceptionMessage ? ex.Message : ""
                , title: title
                , detail: ShowExceptionDetail ? ex.ToString() : "");
        }

        public static void Alert(string message, LogLevel level = LogLevel.Information, Exception exception = null, string detail = null, string title = null, AlertFlags flags = AlertFlags.None, IEnumerable<AlertButton> buttons = null)
        {
            Alert alert = new Alert()
            {
                Message = message,
                LogLevel = level,
                Exception = exception,
                Detail = detail,
                Title = title,
                Flags = flags,
                Buttons = buttons,
            };
            Alert(alert);

        }
        private static ILogger l = Log.Get();

#if !NET35
        public static async Task<bool> Ask(string message)
        {
#if UNITY // TOUNITY
            throw new NotImplementedException();
#else
            ManualResetEventSlim ev = new ManualResetEventSlim();
            bool result = false;

            DependencyContext.Current.GetService<ILionFireShell>().BeginInvoke(new Action(() =>
            //var x = LionFire.Applications.LionFireApp.DefaultDispatcher.BeginInvoke(new Action(() =>
            {

                Alert alert = new Alert()
                {
                    Message = message,
                    LogLevel = LogLevel.Information,
                    //Exception = exception,
                    //Detail = detail,
                    //Title = title,
                    Flags = AlertFlags.Modal | AlertFlags.MustAcknowledge,
                    Buttons = new AlertButton[] { new AlertButton("Yes", () => { result = true; ev.Set(); }), new AlertButton("No", () => ev.Set()) },
                };
                Alert(alert);
            }));

            await Task.Run(() =>
                {
                    int waitTime = 5000;
                    int totalWaitTime = 0;

                    while (!ev.Wait(waitTime))
                    {
                        totalWaitTime += waitTime;
                        l.Trace("Waiting for user response: " + message);
                    }
                });

            return result;
#endif
        }


        /// <summary>
        /// Returns null if canceled, otherwise the string the user entered
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<string> AskString(string message, string okButtonText = "Ok", string startingText = null)
        {
#if UNITY // TOUNITY
            throw new NotImplementedException();
#else
            ManualResetEventSlim ev = new ManualResetEventSlim();
            //bool result = false;
            string result = null;

           DependencyContext.Current.GetService<ILionFireShell>().BeginInvoke(new Action(() =>
           {
               DialogResult dialogResult = new DialogResult()
               {
                   TextEntry = startingText,
               };

               Alert alert = new Alert()
               {
                   DialogResult = dialogResult,
                   Message = message,
                   LogLevel = LogLevel.Information,
                    //Exception = exception,
                    //Detail = detail,
                    //Title = title,
                    Flags = AlertFlags.Modal | AlertFlags.MustAcknowledge | AlertFlags.TextEntry,
                   Buttons = new AlertButton[] { new AlertButton(okButtonText, () => { result = dialogResult.TextEntry; ev.Set(); }), new AlertButton("Cancel", () => ev.Set()) },
               };
               Alert(alert);
           }));

            await Task.Run(() =>
            {
                int waitTime = 5000;
                int totalWaitTime = 0;

                while (!ev.Wait(waitTime))
                {
                    totalWaitTime += waitTime;
                    l.Trace("Waiting for user response: " + message);
                }
            });

            return result;
#endif
        }
#endif
    }

}
