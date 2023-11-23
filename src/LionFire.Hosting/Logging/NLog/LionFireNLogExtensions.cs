//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using NLog.Extensions.Logging;

//namespace LionFire.Hosting
//{
//    // TODO: Spin off into another DLL or contribute upstream, to avoid dependency on NLog.Extensions.Logging

//    public static class LionFireNLogExtensions
//    {
//        public static ILoggingBuilder AddLionFireNLog(this ILoggingBuilder builder, IConfiguration configuration, string nLogSection = "Logging:NLog")
//            // Was going to do this but it looks like it is mostly done: Contribute to NLog.Extensions.Logging to avoid setting NLog.LogManager.Configuration.  Configure via AddNLog, optionally specify "Logging:NLog".
//            //NLog.LogManager.Configuration = new NLogLoggingConfiguration(configuration.GetSection("Logging:NLog"));
//            //builder.AddNLog();
//            => nLogSection == null
//            ? builder.AddNLog(configuration)
//            //? builder.AddNLog(new NLogProviderOptions
//            //{
//            //    //LoggingConfigurationSectionName = "NLog",
//            //})
//            : builder.AddNLog(configuration.GetSection(nLogSection));
//    }
//}
