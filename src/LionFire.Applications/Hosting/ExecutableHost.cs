using System;
using System.Collections.Generic;
using LionFire.Composables;
using LionFire.Execution.Executables;
using LionFire.MultiTyping;

namespace LionFire.Execution
{
    // FUTURE: Modular init/run strategies?
    // - Strat 1: IInitializable/IStartable
    // - Strat 2: IStateMachine with TransitionKind attributes indicating init/run path
    public class ExecutablesHost<T> : ExecutableBase, IComposable<T>, IExecutable2
        where T : ExecutablesHost<T>
    {

        #region IComposable Implementation

        IEnumerable<object> IComposition.Children => children;
        protected List<object> children = new List<object>();

        public virtual T Add<TComponent>(TComponent component)
            where TComponent : class
        {
            //// REVIEW - only do this block if not added?
            //if (component is IConfigures<IServiceCollection> csc)
            //{
            //    csc.Configure(this.ServiceCollection);
            //}

            if (!children.Contains(component))
            {
                if (component is IAdding adding)
                {
                    if (adding.OnAdding(this))
                    {
                        children.Add(component);
                        //multiType.SetType<T>(component);
                    }
                }
                else
                {
                    children.Add(component);
                    //multiType.AddType<TComponent>(component);
                }
            }
            return (T)this;
        }

        #endregion

    }
}
