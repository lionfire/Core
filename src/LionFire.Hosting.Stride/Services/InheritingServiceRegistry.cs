#nullable enable
using Stride.Core;
using Stride.Core.Annotations;

namespace LionFire.Stride_.Core;

public class InheritingServiceRegistry : IServiceRegistry
{
    #region Dependencies

    public IServiceRegistry Parent { get; }

    #endregion

    #region Components

    protected ServiceRegistry Self { get; } = new();

    #endregion

    #region Lifecycle

    public InheritingServiceRegistry(IServiceRegistry parent)
    {
        Parent = parent;
    }

    #endregion


    #region Methods

    public void AddService<T>([NotNull] T service) where T : class => Self.AddService<T>(service);
    public void RemoveService<T>() where T : class => Self.RemoveService<T>();

    public T GetService<T>() where T : class => Self.GetService<T>() ?? Parent.GetService<T>();
    //public object? GetService(Type type) => Self.GetService(type) ?? Parent.GetService(type);

    #endregion

    #region Events

    public event EventHandler<ServiceEventArgs> ServiceAdded { add { Self.ServiceAdded += value; } remove { Self.ServiceAdded -= value; } }
    public event EventHandler<ServiceEventArgs> ServiceRemoved { add { Self.ServiceRemoved += value; } remove { Self.ServiceRemoved -= value; } }
    
    #endregion
}
