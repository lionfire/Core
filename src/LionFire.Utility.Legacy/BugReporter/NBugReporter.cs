#if NBug

using NBug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace LionFire.Applications
{
    public class NBugReporter : IBugReporter
    {
        private static ILogger l = Log.Get();
		
        #region IsEnabled

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled == value) return; isEnabled = value;

                if (isEnabled)
                {
                    OnEnabling(); // Do this before attaching events!

                    l.Trace(this.GetType().Name + " enabled");
                    
                    AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
                    //Application.Current.DispatcherUnhandledException += NBug.Handler.DispatcherUnhandledException;

                    NBug.Settings.InternalLogWritten += Settings_InternalLogWritten;
                }
                else
                {
                    l.Trace(this.GetType().Name + " disabled");
                    AppDomain.CurrentDomain.UnhandledException -= NBug.Handler.UnhandledException;
                    NBug.Settings.InternalLogWritten -= Settings_InternalLogWritten;
                }
            }
        } private bool isEnabled;

        public static string BugReportUrl = "https://lionfire.ca/crash/nbug/submit.php";

        protected virtual void Configure()
        {
            // Uncomment the following after testing to see that NBug is working as configured
            //NBug.Settings.ReleaseMode = true;

            NBug.Settings.SleepBeforeSend = 10;
            NBug.Settings.StopReportingAfter = 60;
            NBug.Settings.MiniDumpType = NBug.Enums.MiniDumpType.Tiny;
            NBug.Settings.WriteLogToDisk = false;

            // NBug configuration (you can also choose to create xml configuration file)
            NBug.Settings.StoragePath = NBug.Enums.StoragePath.IsolatedStorage;
            NBug.Settings.UIMode = NBug.Enums.UIMode.Full;

            var url = BugReportUrl; // HARDCODE
            //http://lionfire.ca/nbug/"+LionEnvironment.LionAppName

            //NBug.Settings.Destinations.Add(
            //    new NBug.Core.Submission.Web.Http("Url="+url+";")
            //    );

            NBug.Settings.AddDestinationFromConnectionString("Type=Http;Url=" + url + ";");

            //NBug.Settings.Destinations.Add(new NBug.Core.Submission.Web.Mail(
            //    "From="
            //    + LionEnvironment.LionAppName
            //    + "@apps.lionfire.ca;To=nbug."
            //    + LionEnvironment.LionAppName
            //        + "@lionfire.ca;SmtpServer=smtp.lionfire.ca;"));

            //NBug.Settings.Destinations.Add(new NBug.Core.Submission.Web.Mail(
            //    "From="
            //    + LionEnvironment.LionAppName
            //    + "@apps.lionfire.ca;To=jared@lionfire.ca;SmtpServer=smtp.lionfire.ca;"));

            //NBug.Settings.AddDestinationFromConnectionString("Type=Mail;");

            //NBug.Core.Submission.Tracker.Redmine
        }

        protected virtual void OnEnabling()
        {
            Configure();
        }

        void Settings_InternalLogWritten(string arg1, NBug.Enums.LoggerCategory arg2)
        {
            switch (arg2)
            {
                case NBug.Enums.LoggerCategory.NBugError:
                    l.Error(arg1);
                    break;
                case NBug.Enums.LoggerCategory.NBugInfo:
                    l.Info(arg1);
                    break;
                case NBug.Enums.LoggerCategory.NBugTrace:
                    l.Trace(arg1);
                    break;
                case NBug.Enums.LoggerCategory.NBugWarning:
                    l.Warn(arg1);
                    break;
                default:
                    l.Fatal(arg1);
                    break;
            }
        }

        #endregion


        #region Event Handlers

        public bool OnApplicationDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            if (IsEnabled)
            {
                NBug.Handler.DispatcherUnhandledException(sender, args);
                return true;
            }
            return false;
        }

        #endregion

    }
}

#endif