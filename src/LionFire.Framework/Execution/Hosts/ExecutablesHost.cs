using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LionFire.Collections;
using LionFire.Execution.Executables;

namespace LionFire.Execution.Hosts
{
    public class ExecutablesHost : ExecutableBase, ICollection<IExecutable2>
    {
        #region Construction

        public ExecutablesHost()
        {
            faultedExecutablesRO = new ReadOnlyObservableDictionary<IExecutable2, ExecutionGoal>(faultedExecutables);
            transitioningExecutablesRO = new ReadOnlyObservableDictionary<IExecutable2, ExecutionGoal>(transitioningExecutables);
        }

        #endregion

        #region Collections

        ObservableCollection<IExecutable2> hostedExecutables = new ObservableCollection<IExecutable2>();

        public IReadOnlyObservableDictionary<IExecutable2, ExecutionGoal> FaultedExecutables
        {
            get
            {
                return faultedExecutablesRO;
            }
        }
        ObservableDictionary<IExecutable2, ExecutionGoal> faultedExecutables = new ObservableDictionary<IExecutable2, ExecutionGoal>();
        ReadOnlyObservableDictionary<IExecutable2, ExecutionGoal> faultedExecutablesRO;

        public IReadOnlyObservableDictionary<IExecutable2, ExecutionGoal> TransitioningExecutables
        {
            get
            {
                return transitioningExecutablesRO;
            }
        }

        ObservableDictionary<IExecutable2, ExecutionGoal> transitioningExecutables = new ObservableDictionary<IExecutable2, ExecutionGoal>();
        ReadOnlyObservableDictionary<IExecutable2, ExecutionGoal> transitioningExecutablesRO;

        #endregion

        #region (Protected) Event Handlers

        protected virtual void OnAdded(IExecutable2 exe)
        {
            if (exe.StateMachine().CurrentState != this.StateMachine().CurrentState)
            {
                transitioningExecutables.Add(exe, new ExecutionGoal { Executable = exe, DesiredState = this.StateMachine().CurrentState });
                exe.TransitionToState(this.StateMachine().CurrentState);
            }
        }

        #endregion

        #region ICollection Implementation

        public int Count => hostedExecutables.Count;

        public bool IsReadOnly => false;

        public void Add(IExecutable2 exe)
        {
            try
            {
                hostedExecutables.Add(exe);
            }
            catch
            {
                Remove(exe);
                throw;
            }
        }

        public bool Remove(IExecutable2 exe)
        {
            var r1 = hostedExecutables.Remove(exe);
            var r2 = faultedExecutables.Remove(exe);
            var r3 = transitioningExecutables.Remove(exe);
            return r1 || r2 || r3;
        }

        public void Clear()
        {
            hostedExecutables.Clear();
            faultedExecutables.Clear();
            transitioningExecutables.Clear();
        }

        public bool Contains(IExecutable2 item)
        {
            return hostedExecutables.Contains(item);
        }

        public void CopyTo(IExecutable2[] array, int arrayIndex)
        {
            hostedExecutables.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IExecutable2> GetEnumerator()
        {
            return hostedExecutables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
