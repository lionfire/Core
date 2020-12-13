#nullable enable
using LionFire.DependencyInjection;
using LionFire.DependencyMachines;
using LionFire.Ontology;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Hosting
{
    public static class DependencyMachineDelegateHelpers
    {
        public static Func<TParticipant, CancellationToken, Task<object?>> CreateInvoker<TParticipant>(Delegate d, params (Type, Func<IServiceProvider, object>)[] additionalServices)
            where TParticipant : IParticipant, IHas<IServiceProvider>
        {
            var returnType = d.GetType().GetMethod("Invoke").ReturnType;

            if (returnType == typeof(Task<object>))
            {
                return (participant, ct) =>
                {

                    var result = InvokationUtilities.TryInvoke(((IHas<IServiceProvider>)participant).Object, d, additionalServices);
                    return result.missingDependency != null ? Task.FromResult<object?>(result.missingDependency) : (Task<object?>)result.result!;
                };
            }
            else if (returnType == typeof(object))
            {
                return (participant, ct) =>
                {
                    var result = InvokationUtilities.TryInvoke(((IHas<IServiceProvider>)participant).Object, d, additionalServices);
                    return Task.FromResult<object?>(result.missingDependency != null ? result.missingDependency : result.result!);
                };
            }
            else if (returnType == typeof(void))
            {
                return (participant, ct) =>
                {
                    var result = InvokationUtilities.TryInvoke(((IHas<IServiceProvider>)participant).Object, d, additionalServices);
                    return Task.FromResult<object?>(result.missingDependency != null ? result.missingDependency : null);
                };
            }
            else
            {
                throw new ArgumentException($"Delegate must have return type of Task<object> (preferred), object, or void.");
            }
        }
    }
}
