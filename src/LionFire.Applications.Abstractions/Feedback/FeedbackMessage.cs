using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Applications.Feedback
{
    public class FeedbackMessage
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string EmailFrom
        {
            get
            {
                var name = Name;
                if (String.IsNullOrWhiteSpace(name)) { name = "In-App Feedback Form"; }

                var email = Email;
                //if (String.IsNullOrWhiteSpace(Email)) { email = DefaultEmailFrom; }

                return name + $"<{email}>";
            }
        }
        public string Version { get; set; }
        public string Message { get; set; }

        public string Subject { get; set; }
    }
}
