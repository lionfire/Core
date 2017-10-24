using LionFire.DependencyInjection;
using System;
using System.Threading.Tasks;
using LionFire.Validation;
using System.Reflection;

namespace LionFire.Execution.Executables
{
    // REFACTOR - Use a visitor pattern instead of inheritance, ENH - some sort of elegant state machine with opt-in to Initializable logic
    public class InitializableExecutableBase : ExecutableExBase
    {

        public virtual bool CanInitializeAfterDispose => false;

        //private void ValidateCanInitialize()
        //{
        //    //switch (State)
        //    //{
        //    //    case ExecutionState.Unspecified:
        //    //    case ExecutionState.Uninitialized:
        //    //        break;
        //    //    //case ExecutionState.Initializing:
        //    //    //    break;
        //    //    //case ExecutionState.Ready:
        //    //    //    break;
        //    //    //case ExecutionState.Starting:
        //    //    //    break;
        //    //    //case ExecutionState.Started:
        //    //    //    break;
        //    //    //case ExecutionState.Pausing:
        //    //    //    break;
        //    //    //case ExecutionState.Paused:
        //    //    //    break;
        //    //    //case ExecutionState.Unpausing:
        //    //    //    break;
        //    //    //case ExecutionState.Stopping:
        //    //    //    break;
        //    //    //case ExecutionState.Stopped:
        //    //    //    break;
        //    //    //case ExecutionState.Faulted:
        //    //    //case ExecutionState.Finished:
        //    //    case ExecutionState.Disposed:
        //    //        return false;
        //    //    default:
        //    //        return true;
        //    //}
        //}
        

        public async Task<ValidationContext> Initialize()
        {
            ValidationContext validationContext = null;
            Func<ValidationContext> vcGetter = () => { if (validationContext == null) validationContext = new ValidationContext(); return validationContext; };

            if (!this.IsInitialized())
            {
                //ValidateCanInitialize();  

                SetState (State,ExecutionStateEx.Initializing);
                if (!CanInitializeAfterDispose && State == ExecutionStateEx.Disposed) throw new ObjectDisposedException(this.GetType().Name);
                //StateChangeContext c = new StateChangeContext();
                await OnInitializing(vcGetter);
                if (validationContext.IsValid()) State = ExecutionStateEx.Ready;
            }

            return validationContext;
        }

        protected virtual Task OnInitializing(Func<ValidationContext> validationContext)
        {
            if (GetType().GetTypeInfo().GetCustomAttribute<HasDependenciesAttribute>() != null)
            {
                this.TryResolveDependencies(validationContext);
            }
            return Task.CompletedTask;
        }

    }


}
