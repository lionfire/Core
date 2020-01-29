#if UNUSED // INCOMPLETE - no point yet?
using LionFire.Execution;
using LionFire.Dependencies;
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

    /// <summary>
    /// INCOMPLETE - no point yet?
    /// </summary>
    public class ExecutionContainer : ExecutablesHost<ExecutionContainer>, IReadOnlyMultiTyped
    {
        protected readonly MultiType multiType = new MultiType();

        // REVIEW - Not sure this is needed or a good idea
        T IReadOnlyMultiTyped.AsType<T>()
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


        public DependencyContext DependencyContext { get; private set; } = new DependencyContext();

        public override ExecutionContainer Add<TComponent>(TComponent component)
            //where TComponent : class
        {
#if true
            throw new Exception("TODO REVIEW: Add adds to multiType in this class and children in the base class.  Is that right?");
#else
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
#endif
        }


    }


}
#endif