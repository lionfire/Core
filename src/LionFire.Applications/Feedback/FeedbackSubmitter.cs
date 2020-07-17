using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Applications.Feedback
{
    public class FeedbackSubmitter : IFeedbackSubmitter
    {
        //public static string DefaultEmailFrom => $"appfeedback+{AppInfo.Instance.AppName}@lionfire.ca";

        public Task<bool> Submit(FeedbackMessage msg)
        {
            throw new NotImplementedException();
        }
}
}
