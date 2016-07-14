using LionFire.Execution.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    
    public class ExecutionContext
    {
        public Guid Guid { get; } = Guid.NewGuid();

        #region Relationships

        public object Parent { get; set; }

        #endregion

        #region Construction

        public ExecutionContext() { }
        public ExecutionContext(string specifier) { this.Config = new ExecutionConfig(specifier); }

        #endregion

        #region Parameters

        public ExecutionConfig Config { get; set; }

        #endregion

        #region State

        #region Execution Target

        public string ExecutionRootDir { get; set; }
        public string ExecutionPath { get; set; }

#region ExecutionObject

        /// <summary>
        /// An optional object holding a reference to the primary object or proxy to the item being executed.
        /// This could be the instantiation of a .NET class, a proxy to a class, or a scripting context.
        /// </summary>
        public object ExecutionObject { get; set; }

#region Derived Object

        public IExecutable Executable { get { return ExecutionObject as IExecutable; } }

        public IDisposable DisposableExecutionObject { get { return ExecutionObject as IDisposable; } }

        public bool IsDisposable {
            get { return DisposableExecutionObject != null; }
        }

        #endregion


        public string ExecutionObjectMimeType { get; set; }

        #endregion

#endregion


        public IExecutionController Controller { get; set; }

        public ExecutionContextInitializationState InitializationState { get; set; }

        #region Status

        public ExecutionHostState Status {
            get { return state; }
            set { state = value; }
        }
        private ExecutionHostState state;

        #endregion

        // Consider bringing other things from Task
        public AggregateException Exception { get; set; }
        public Task RunTask { get; internal set; }

        #endregion




        //public string Runtime { get; set; }
        //public string RuntimeVersion { get; set; }



    }
}
