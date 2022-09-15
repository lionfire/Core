using LionFire.Types.Scanning;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Threading;

namespace LionFire.Mvvm;

public class ViewModelTypeRegistry : IHostedService, IDisposable
{
    public IReadOnlyDictionary<Type, Type> ViewModelsByModelType => viewModelsByModelType;
    Dictionary<Type, Type> viewModelsByModelType = new();

    public IOptionsMonitor<ViewModelConfiguration> OptionsMonitor { get; }

    ManualResetEventSlim Ready = new();

    #region Lifecycle

    public ViewModelTypeRegistry(IOptionsMonitor<ViewModelConfiguration> optionsMonitor)
    {
        OptionsMonitor = optionsMonitor;
    }

    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(100); // We can probably wait this long for view models
        Init();
    }

    public IEnumerable<Assembly> AssembliesToScan
        => OptionsMonitor.CurrentValue.TypeScanOptions.GetAssemblies();

    private void Init()
    {
        try
        {
            Ready.Reset();

            var dict = new Dictionary<Type, Type>();

            foreach (var assembly in AssembliesToScan)
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var iface in type.GetInterfaces())
                    {
                        if(iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IViewModel<>))
                        {
                            var genericArguments = iface.GetGenericArguments();
                            Register(type, genericArguments[0]);
                        }
                    }
                }
            }
        }
        finally
        {
            Ready.Set();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        viewModelsByModelType = null;
    }

    #endregion

    public void Register(Type model, Type viewModel)
    {
        viewModelsByModelType.Add(model, viewModel);
    }

    public Type GetViewModelType(Type modelType)
    {
        Ready.Wait();
        return ViewModelsByModelType[modelType];
    }
}
