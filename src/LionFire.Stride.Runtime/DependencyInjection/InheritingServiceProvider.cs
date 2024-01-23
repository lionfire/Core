namespace LionFire.Hosting;

public class InheritingServiceProvider : IServiceProvider
{
    #region Dependencies

    public IServiceProvider Parent { get; }
    public IServiceProvider Child { get; }

    #endregion

    #region Lifecycle

    public InheritingServiceProvider(IServiceProvider parent, IServiceProvider child)
    {
        Parent = parent;
        Child = child;
    }

    #endregion

    #region Methods

    public object? GetService(Type type) => Child.GetService(type) ?? Parent.GetService(type);

    #endregion
}
