using LionFire.Execution;
using LionFire.DependencyInjection;
using LionFire.Composables;
using LionFire.MultiTyping;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Applications.Hosting
{
#if false
    public interface IExecutionStrategy
    {
    }

    public class InterfacesExecutionStrategy : IExecutionStrategy
    {

    }

    public class StateMachineExecutionStrategy : IExecutionStrategy
    {
        public IExecutable2 Target { get; set; }
        public IEnumerable<Func<object, Task<object>>> GetInitializers()
        {
            {
                var hsm = (IHas<IStateMachine<ExecutionState2, ExecutionTransition>>)Target;
                var sm = hsm?.Object;
                
            }
        }

    }
#endif

    public class ExecutionContainer : ExecutablesHost<ExecutionContainer>, IReadonlyMultiTyped
    {
        protected readonly MultiType multiType = new MultiType();

        // REVIEW - Not sure this is needed or a good idea
        T IReadonlyMultiTyped.AsType<T>()
        {
            switch (typeof(T).Name)
            {
                //case nameof(IServiceCollection):
                //    return (T)ServiceCollection;
                //case nameof(IServiceProvider):
                //    return (T)ServiceProvider;
                default:
                    var result = multiType.AsType<T>();
                    if (result != null) return result;

                    //if (ServiceProvider == null)
                    //{
                    //    return null;
                    //}
                    //else
                    //{
                    //    return ServiceProvider.GetService<T>();
                    //}
                    return null;
            }
        }


        public InjectionContext InjectionContext { get; private set; } = new InjectionContext();

        public ExecutionContainer Add<TComponent>(TComponent component)
      where TComponent : class
        {
            base.Add(component);
            //// REVIEW - only do this block if not added?
            //if (component is IConfigures<IServiceCollection> csc)
            //{
            //    csc.Configure(this.ServiceCollection);
            //}

            if (component is IAdding adding)
            {
                if (adding.OnAdding(this))
                {
                    multiType.AddType<TComponent>(component);
                }
            }
            else
            {
                multiType.AddType<TComponent>(component);
            }
            return this;
        }


    }


}
