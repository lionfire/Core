using Microsoft.Extensions.Hosting;
using JasperFx.CommandLine;
using JasperFx.CommandLine.Descriptions;

namespace LionFire.Oakton;

public class MultiCommandProgramBuilder
{
    public IHostBuilder? HostBuilder { get; }
    public IHost? Host { get; }

    Dictionary<string, MultiCommand> Commands = new Dictionary<string, MultiCommand>();

#if TODO // Wait for Oakton support? or dig out the private IHostBuilder from HostApplicationBuilder?  Or use adapter?
    public MultiCommandProgramBuilder(HostApplicationBuilder builder)
    {
        HostBuilder = builder;
    }
#endif
    public MultiCommandProgramBuilder(IHostBuilder builder)
    {
        HostBuilder = builder;
    }
    public MultiCommandProgramBuilder(IHost host)
    {
        Host = host;
    }

    public MultiCommandProgramBuilder Command(string name, params Type[] commandTypes)
    {
        Commands.Add(name, new MultiCommand(commandTypes));
        return this;
    }

    public async Task<int> Run(string[] args)
    {
        if (args.Length == 0 || !Commands.ContainsKey(args[0]) || args[0] == "help" || args[0] == "--help")
        {
            ShowHelp();
            return 0;
        }

        var commandName = args[0];
        args = args.Skip(1).ToArray();

        var command = Commands[commandName];
        command.HostBuilder = HostBuilder;

        IServiceProvider? serviceProvider = Host?.Services;
        var executor = command.GetCommandExecutor(serviceProvider);
        
        return await executor.ExecuteAsync(args);
    }

    private void ShowHelp()
    {
        Console.WriteLine();
        Console.WriteLine("The available commands are:");
        Console.WriteLine();
        Console.WriteLine("  Alias          Description");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        foreach (var kvp in Commands)
        {
            var command = kvp.Value;
            foreach (var commandType in command.Types)
            {
                // Get command attributes for description and name
                // NOTE: AreaAttribute doesn't exist in current Oakton version
                // var areaAttr = commandType.GetCustomAttributes(typeof(AreaAttribute), false).FirstOrDefault() as AreaAttribute;
                var descAttr = commandType.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).FirstOrDefault() as System.ComponentModel.DescriptionAttribute;
                
                string commandAlias = commandType.Name.Replace("Command", "").ToLower();
                
                var description = descAttr?.Description ?? "No description available";
                
                Console.WriteLine($"  {commandAlias.PadRight(12)} {description}");
            }
        }

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("Use dotnet run -- ? [command name] or dotnet run -- help [command name] to see usage help about a specific command");
    }
}