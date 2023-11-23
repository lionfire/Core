#nullable enable

namespace LionFire.Types.Scanning;

//public class TypeScanService : IHostedService, IDisposable
//{
//    public Dictionary<string, TypeScanJob> Jobs { get; private set; } = new();

//    public void Dispose()
//    {
//        Jobs = null;
//    }

//    public Task<IEnumerable<Type>> ScanForTypes<TInterface>(TypeScanOptions options)
//    {
//        return Task.Run(async () =>
//        {
//            var job = new TypeScanJob(options);
//            return await job.Run();
//        });
//    }

//    public Task StartAsync(CancellationToken cancellationToken)
//    {
//        throw new NotImplementedException();
//    }

//    public Task StopAsync(CancellationToken cancellationToken)
//    {
//        Dispose();
//        return Task.CompletedTask;
//    }
//}

using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public class TypeScanner
{
    TypeScannerOptions Options
    {
        get => options;
        set
        {
            options = value;
            options.Validate();
        }
    }
    private TypeScannerOptions options;

    public TypeScanner(IOptionsMonitor<TypeScannerOptions> optionsMonitor)
    {
        Options = optionsMonitor.CurrentValue;
    }


    public static IEnumerable<Type> GetAvailableTypes<T>(Func<Type, bool> typePredicate, Func<Assembly, bool> assemblyPredicate)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assemblyPredicate))
        {
            foreach (var type in assembly.GetTypes().Where(typePredicate))
            {
                yield return type;
            }
        }
    }

    public Func<IEnumerable<Assembly>>? AssembliesToScanFunc { get; set; }
    public static Func<IEnumerable<Assembly>> DefaultAssembliesToScanFunc = () => AppDomain.CurrentDomain.GetAssemblies();
    public IEnumerable<Assembly> AssembliesToScan
        => assembliesToScan ??= (AssembliesToScanFunc?.Invoke() ?? DefaultAssembliesToScanFunc());
    IEnumerable<Assembly> assembliesToScan;

    public void ResetCache()
    {
        assembliesToScan = null;
    }

    public IEnumerable<Type> GetAllAssignableTo<TInterface>(string test, Func<Type, bool> typeFilter )
    {
        Debug.WriteLine("test", test);
        if (typeFilter == null) throw new ArgumentNullException();
        foreach (var assembly in AssembliesToScan.Where(a => Options.PassesFilter(a)))
        {
            foreach (var type in assembly.GetTypes()
                .Where(t =>
                    t != typeof(TInterface)
                    && t.IsAssignableTo(typeof(TInterface))
                ))
            {
                if (typeFilter != null && !typeFilter(type)) { continue; }
                yield return type;
            }
        }
    }
}