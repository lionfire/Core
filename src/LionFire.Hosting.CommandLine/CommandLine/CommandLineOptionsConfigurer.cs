#nullable enable
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine.Binding;
using System.CommandLine.NamingConventionBinder;

namespace LionFire.Hosting.CommandLine;

public static class CommandLineOptionsConfigurer<T>
    where T : class
{
    public static void Configure(T optionsObject, Action<Action<IServiceCollection>> configureServices, BindingContext bindingContext)
    {
        update(optionsObject);

        configureServices(s => s.Configure<T>(o => { update(o); }) );

        void update(T o)
        {
            var binder = new ModelBinder(typeof(T));
            binder.UpdateInstance(o, bindingContext);

            ConfigureAction?.Invoke(o, bindingContext);
        }
    }
    //public static void DoIt(ConfigurationManager configuration, BindingContext bindingContext)
    //{
    //    configureServices(s =>
    //        s.Configure<T>(o =>
    //        {
    //            var binder = new ModelBinder(typeof(T));
    //            binder.UpdateInstance(o, bindingContext);

    //            Configure?.Invoke(o, bindingContext);
    //        })
    //    );
    //}

    public static Action<T, BindingContext>? ConfigureAction { get; set; }
}
