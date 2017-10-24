using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Execution
{
    //// Not Recommended - or is there a place for this?  Ensure dependencies are init during initialization
    //public class DependencyExecutable : ExecutableBase, IHasDependencies, IRequiresServices
    //{
    //    #region Dependencies
    //    public IServiceProvider ServiceProvider {
    //        get { return serviceProvider; }
    //        set {
    //            serviceProvider = value;
    //        }
    //    }
    //    protected IServiceProvider serviceProvider;

    //    #endregion

    //    public UnsatisfiedDependencies UnsatisfiedDependencies {
    //        get {
    //            return unsatisfiedDependencies;
    //        }
    //    }
    //    protected UnsatisfiedDependencies unsatisfiedDependencies = null;

    //    /// <summary>
    //    /// Invokes this.TryResolveDependencies(ref unsatisfiedDependencies, ServiceProvider).  
    //    /// </summary>
    //    /// <returns>True if there are no unresolved dependencies</returns>
    //    protected virtual bool TryResolveDependencies()
    //    {
    //        return this.TryResolveDependencies(ref unsatisfiedDependencies, ServiceProvider);
    //    }
    //}
}
