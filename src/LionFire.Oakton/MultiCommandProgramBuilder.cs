using Microsoft.Extensions.Hosting;
using Oakton;

namespace LionFire.Oakton
{
    public class MultiCommandProgramBuilder
    {
        public IHostBuilder? HostBuilder { get; }
        public IHost? Host { get; }

        Dictionary<string, MultiCommand> Commands = new Dictionary<string, MultiCommand>();

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
                ShowHelp();

            var commandName = args[0];
            args = args.Skip(1).ToArray();

            var command = Commands[commandName];
            command.HostBuilder = HostBuilder;

            IServiceProvider? serviceProvider = null;
            var executor = command.GetCommandExecutor(serviceProvider);
            
            return await executor.ExecuteAsync(args);
        }

        private void ShowHelp()
        {
            throw new NotImplementedException();
            //ConsoleWriter.Write("Help text here");
            Environment.Exit(1);
        }
    }
}