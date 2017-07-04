using LionFire.DependencyInjection;
using System;
using System.Threading.Tasks;
using LionFire.Validation;
using System.Reflection;

namespace LionFire.Execution.Executables
{
    public class InitializableExecutableBase : ExecutableBase
    {

        public virtual bool CanInitializeAfterDispose => false;

        public async Task<ValidationContext> Initialize()
        {
            ValidationContext validationContext = null;
            if (!this.IsInitailized())
            {
                if (!CanInitializeAfterDispose && State == ExecutionState.Disposed) throw new ObjectDisposedException(this.GetType().Name);
                await OnInitializing(ref validationContext);
                if (validationContext.IsValid()) State = ExecutionState.Ready;
            }

            return validationContext;
        }

        protected virtual Task OnInitializing(ref ValidationContext validationContext)
        {
            if (GetType().GetTypeInfo().GetCustomAttribute<HasDependenciesAttribute>() != null)
            {
                this.TryResolveDependencies(ref validationContext);
            }
            return Task.CompletedTask;
        }

    }


}
