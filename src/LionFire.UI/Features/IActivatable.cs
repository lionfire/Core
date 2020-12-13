namespace LionFire.UI
{
    public interface IActivated
    {
        bool Active { get; }
    }
    public interface IActivatable : IActivated
    {
        new bool Active { get; set; }
    }
}
