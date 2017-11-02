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

        IEnumerable<object> IComposition.Children => throw new NotImplementedException();
        protected List<object> children = new List<object>();

        //T IComposable<T>.Add<TComponent>(TComponent component)
        //{
        //    children.Add(component);
        //    return (T) this;
        //}

        public T Add<TComponent>(TComponent component)
            where TComponent : class
        {
            //// REVIEW - only do this block if not added?
            //if (component is IConfigures<IServiceCollection> csc)
            //{
            //    csc.Configure(this.ServiceCollection);
            //}

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
            return (T)this;
        }

        #endregion



    }
}
