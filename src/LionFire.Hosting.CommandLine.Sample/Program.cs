
using LionFire.Hosting.CommandLine;

// TODO: Read https://github.com/dotnet/command-line-api/blob/main/src/System.CommandLine.Hosting.Tests/HostingHandlerTest.cs
// TODO: Read https://github.com/dotnet/command-line-api/blob/main/src/System.CommandLine.Hosting.Tests/HostingTests.cs

return new HostApplicationBuilderProgram()
       //.DefaultArgs("run")
       //.RootCommand(b => b.LionFireAppInfo())
       //.AddOrleansCommands()
   //.Command<UniverseSiloCommandLineOptions>("run", (commandContext, builder) => builder
   //.If(commandContext.GetOptions<UniverseSiloCommandLineOptions>().Dev, b => b.Environment.EnvironmentName = "Development")
   //.LionFire(7180, l => l
   .RunAsync(args).Result;
