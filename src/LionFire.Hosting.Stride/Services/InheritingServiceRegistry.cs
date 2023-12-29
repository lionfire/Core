using Stride.Core;
using Stride.Core.Annotations;

namespace LionFire.Hosting;

public class InheritingServiceRegistry : IServiceRegistry
{
    #region Dependencies

    public IServiceRegistry Parent { get; }

    #endregion

    #region Lifecycle

    public InheritingServiceRegistry(IServiceRegistry parent)
    {
        Parent = parent;
    }

    #endregion

    #region State

    protected ServiceRegistry Self { get; } = new();

    #endregion

    #region Methods

    public void AddService<T>([NotNull] T service) where T : class => Self.AddService<T>(service);
    public void RemoveService<T>() where T : class => Self.RemoveService<T>();

    public T GetService<T>() where T : class => Self.GetService<T>() ?? Parent.GetService<T>();

    #endregion

    #region Events

    public event EventHandler<ServiceEventArgs> ServiceAdded { add { Self.ServiceAdded += value; } remove { Self.ServiceAdded -= value; } }
    public event EventHandler<ServiceEventArgs> ServiceRemoved { add { Self.ServiceRemoved += value; } remove { Self.ServiceRemoved -= value; } }
    
    #endregion
}
