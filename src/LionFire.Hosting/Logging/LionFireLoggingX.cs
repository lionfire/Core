#nullable enable
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using LionFire.Logging.Serilog;
using LionFire.Execution;

namespace LionFire.Hosting;

//public static class LionFireLoggingX
//{

//    //public static LionFireLogBuilder Log(this ILionFireHostBuilder lionFireHostBuilder, Action<LionFireLogBuilder> builder);  // Abstraction idea
//}

