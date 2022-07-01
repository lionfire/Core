using Microsoft.Extensions.Hosting;
using Oakton;

namespace LionFire.Oakton
{
    // Inspired by this Multi-command approach: https://gist.github.com/cocowalla/5b2b23348e482f900927cd46181821ab
    // Discussion: https://github.com/JasperFx/oakton/issues/23

    public class MultiCommand
    {
        public MultiCommand(Type[] commandTypes)
        {
            Types = commandTypes;
        }

        public Type[] Types { get; set; } = Array.Empty<Type>();
        public IHostBuilder? HostBuilder { get; set; }

        public CommandExecutor GetCommandExecutor(IServiceProvider? serviceProvider)
        {
            var executor = CommandExecutor.For(factory =>
            {
                foreach (var type in Types)
                {
                    factory.RegisterCommand(type);
                }

                if (HostBuilder != null)
                {
                    factory.ConfigureRun = cmd =>
                    {
                        if (cmd.Input is IHostBuilderInput i)
                        {
                            factory.ApplyExtensions(HostBuilder); // Applies user-defined extensions which augment IServiceCollection
                            i.HostBuilder = HostBuilder;
                        }
                    };
                }
            }, new ServiceProviderCommandCreator(serviceProvider));

            return executor;
        }
    }
}
