using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Twilio;
using Twilio.Clients;
using Twilio.Exceptions;
using Twilio.Http;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace LionFire.Notifications.Twilio
{
    //public interface INotifier
    //{
    //    void SendAlert(string msg);
    //}
    public interface ISmsNotifier
    {
        Task<SmsAlertResult> SendSmsAlert(string msg);
    }
    public interface IVoiceNotifier
    {
        Task<VoiceAlertResult> SendVoiceAlert(string msg);
    }
    public enum VoiceAlertResultType
    {
        Unspecified = 0,
        NoAnswer = 1 << 1,
        Voicemail = 1 << 2,
        NoAck = 1 << 3,
        Acked = 1 << 4,
        ResponseGiven = 1 << 5,
    }

    public enum SmsAlertResultType
    {
        Unspecified = 0,
        Undelivered = 1 << 1,
        Delivered = 1 << 2,
        Acked = 1 << 3,
        ResponseGiven = 1 << 4,
    }


    public class VoiceAlertResult
    {
        public VoiceAlertResultType VoiceAlertResultType { get; set; }
    }
    public class SmsAlertResult
    {
        public SmsAlertResultType SmsAlertResultType { get; set; }
    }
    public class TwilioNotifier : ISmsNotifier, IVoiceNotifier
    {
        public string VoiceNumber { get; set; } = "";
        public string SmsNumber { get; set; } = "";
        public string NumberSource { get; set; } = "";

        //public void SendAlert(string msg)
        //{
        //}

        public ITwilioRestClient TwilioRestClient
        {
            get
            {
                if (twilioRestClient == null)
                {
                    InitTwilio();
                }
                return twilioRestClient;
            }
        }
        private ITwilioRestClient twilioRestClient;
        private void InitTwilio()
        {
            if (twilioRestClient != null) return;
	    string account = "AC";
	    string key = "";
            TwilioClient.Init(account, key);
            twilioRestClient = TwilioClient.GetRestClient();
        }

        public async Task<VoiceAlertResult> SendVoiceAlert(string msg)
        {
            InitTwilio();
            var to = new PhoneNumber(VoiceNumber);
            var from = new PhoneNumber(NumberSource);

            new CreateCallOptions(to, from)
            {
                
            };

            // Initiate a new outbound call
            try
            {
                var call = CallResource.Create(
                    to,
                    from,

                    url: new Uri("https://demo.twilio.com/docs/voice.xml")
                    //url: new Uri("http://demo.twilio.com/welcome/voice/")
                );
                Console.WriteLine(string.Format($"Started call: {call.Sid}"));
            }
            catch (TwilioException e)
            {

                Console.WriteLine(string.Format($"Error: {e.Message}"));
            }

            //TheSystemIsDown
            //return new VoiceAlertResult()
            //{
            //};
            throw new NotImplementedException();
        }

        public string TheSystemIsDown = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<Response>
    <Say voice = ""alice"" language=""en-CA"">The system is down!  Dollar Yen below 1 0 9 point 0 3 1.  Do you want to short?</Say>
</Response>";

        public async Task<SmsAlertResult> SendSmsAlert(string msg)
        {
            InitTwilio();
            var message = await MessageResource.CreateAsync(
                to: new PhoneNumber(SmsNumber),
                from: new PhoneNumber(NumberSource),
                body: msg);
            if (message.ErrorCode.HasValue)
            {
                
                throw new Exception("SMS failed: " + message.ErrorMessage);
            }
            else
            {
                // TODO: TOLOG
                Debug.WriteLine("SMS succeeded?");
            }

            return new SmsAlertResult()
            {
            };
        }
    }
}
