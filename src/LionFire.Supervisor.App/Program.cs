using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
//using FluentEmail.Mailgun;
using RestSharp;
using RestSharp.Authenticators;

//[assembly:UserSecretsId("LionFire.Supervisor")]
namespace LionFire.Supervisor.App
{

    class Program
    {

        public static string MailgunKey => Configuration["MailgunKey"];

        static IConfigurationRoot Configuration;

        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: lfsup [Path to Executable]");
                return;
            }

            var builder = new ConfigurationBuilder();

            //if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Program>();
            }

            Configuration = builder.Build();

            try
            {
                MailgunAdapter.Init();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error initializing Mailgun: " + ex);
                return;
            }

            var m = new ProcessMonitor(args[0]);
            m.IsEnabled = true;

            while (Console.ReadLine() != "exit") Console.WriteLine("Type exit to exit");
        }
    }

    public static class EmailSettings
    {
        public static string From { get; set; } = "supervisor@example.com";
        public static string To { get; set; } = "me@example.com";

            public static string MailgunDomain = "mg.example.com";
    }


    public static class MailgunAdapter
    {
        //public static MailgunClient Client => client;
        //static MailgunClient client;

        public static void Init()
        {

            var key = Program.MailgunKey;
            Console.WriteLine("Key: " + key);
            if (string.IsNullOrWhiteSpace(key)) throw new Exception("Key not set");


            //var sender = new MailgunSender( "mg.lionfire.ca", key);
            //Email.DefaultSender = sender;

            //client = new MailgunClient(, key, 3);

            //client.SendMail(new MailMessage(EmailSettings.From, EmailSettings.To)
            //{
            //    Subject = "Hello from mailgun",
            //    Body = "this is a test message from mailgun."
            //});

            Send("initialized", "test lfsup");


        }

        public static void Send(string message, string title = null)
        {
            if (title == null) title = message;

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api", "key");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", EmailSettings.MailgunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"Excited User <{EmailSettings.From}>");
            request.AddParameter("to", EmailSettings.To);
            request.AddParameter("subject", title);
            request.AddParameter("text", message);
            request.Method = Method.POST;
            client.ExecuteAsync(request, x => { });
        }
    }


    public class ProcessMonitor
    {
        public string ExePath { get; set; }
        public string ProcessName => Path.GetFileName(ExePath);


        public void Alert(string message, string title = null)
        {
            if (title == null) title = message;
            //MailgunAdapter.Client.SendMail(new MailMessage(EmailSettings.From, EmailSettings.To)
            //{
            //    Subject = $"[LF-Sup] " + title,
            //    Body = message,
            //});
        }

        #region IsUp

        public bool? IsUp
        {
            get { return isUp; }
            set
            {
                if (isUp == value) return;
                var oldVal = isUp;
                isUp = value;
                if (isUp.HasValue)
                {
                    if (isUp.Value)
                    {
                        if (oldVal.HasValue && oldVal.Value == false)
                        {
                            Console.WriteLine("Back up: " + ProcessName);
                        }
                    }
                    else
                    {
                        if (oldVal.HasValue && oldVal.Value == true)
                        {
                            Alert($"DOWN: {ProcessName}@{Environment.MachineName}");
                        }
                    }
                }

            }
        }
        private bool? isUp;

        #endregion


        public ProcessMonitor(string path)
        {

            this.ExePath = path;
            //Timer = new Timer(TimerIntervalMilliseconds)
            //{
            //    AutoReset = true,
            //    Enabled = true,
            //};
            //Timer = new Timer(new TimerCallback(x => OnTimer(x, null)), null, TimerIntervalMilliseconds, TimerIntervalMilliseconds);
            //{
            //    AutoReset = true,
            //    Enabled = true,
            //};

            if (!File.Exists(path))
            {
                throw new ArgumentException($"File not found: {path}");
            }
        }

        public bool IsEnabled
        {
            get => Timer != null;
            set
            {
                if (value)
                {
                    Timer = new Timer(new TimerCallback(x => OnTimer(x)), null, TimerIntervalMilliseconds, TimerIntervalMilliseconds);
                }
                else
                {
                    Timer?.Dispose();
                    Timer = null;
                }
            }
        }

        public static int TimerIntervalMilliseconds = 30000;

        public static Timer Timer ;


        private void OnTimer(object context)
        {
            //Timer.Elapsed += OnTimer;

            int count = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(ExePath)).Length;
            Console.WriteLine("Count: " + count);
            IsUp = count > 0;
        }
    }

}
