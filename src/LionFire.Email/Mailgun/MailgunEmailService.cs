//using FluentEmail.Mailgun;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Threading.Tasks;
//using Email = FluentEmail.Core.Email;

//namespace LionFire.Email.Mailgun
//{

//    public class MailgunEmailService : IEmailService
//    {

//        MailgunConfiguration config;

//        public MailgunEmailService(IConfiguration configuration)
//        {
//            config = configuration.Get<MailgunConfiguration>("Mailgun")
//        }

//        public Task<bool> Send(string from, string to, string subject, string message)
//        {
//            var email = Email
//                .From(from)
//                .To(to)
//                .Subject(subject)
//                .Body(message)
//                .Send();
            
//        }
//    }

//}
