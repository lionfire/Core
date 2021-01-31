using LionFire.Dependencies;
using LionFire.LiveSharp;
using LiveSharp;
using MediatR;
using Microsoft.Extensions.Logging;

[assembly: LiveSharpInject("*")]

namespace LiveSharp 
{
    class LiveSharpDashboard : ILiveSharpDashboard
    {
        #region Optional Dependencies

        static ILogger Logger => DependencyContext.Current?.GetService<ILogger<LiveSharpDashboard>>() ?? (ILogger)LionFire.Logging.Null.NullLogger.Instance;

        static IMediator Mediator => DependencyContext.Current?.GetService<IMediator>();

        #endregion

        // This method will be run during the start of your application and every time you update it
        public void Configure(ILiveSharpRuntime app) 
        {
            app.Config.SetValue("disableBlazorCSS", "false");
            app.UseDefaultBlazorHandler();

            app.OnCodeUpdateReceived(new CodeUpdateHandler(updatedMethods =>
            {
                Logger.LogInformation("[LionFire.Blazor.Components] OnCodeUpdateReceived");

                foreach (var updatedMethod in updatedMethods)
                {
                    Logger.LogInformation($"{typeof(ILiveSharpRuntime).Name}.{nameof(ILiveSharpRuntime.OnCodeUpdateReceived)}() Method: {updatedMethod.DeclaringType.FullName}.{updatedMethod.MethodIdentifier}");

                    Mediator.Publish(new UpdatedMethodNotification(updatedMethod));
                }
            }));

            app.OnResourceUpdateReceived(new ResourceUpdateHandler((path, content) =>
            {
                Logger.LogInformation($"[LionFire.Blazor.Components] {typeof(ILiveSharpRuntime).Name}.{nameof(ILiveSharpRuntime.OnResourceUpdateReceived)}() {path}");

                Mediator.Publish<UpdatedResourceNotification>(new UpdatedResourceNotification(path, content));
            }));

        }





        public void Run(ILiveSharpRuntime app)
        {
            // Use this method to execute any code in runtime
            // Every time you update this method LiveSharp will invoke it
        }
    } 
}