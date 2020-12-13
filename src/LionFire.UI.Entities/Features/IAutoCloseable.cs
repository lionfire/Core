namespace LionFire.UI
{
    public interface IAutoCloseable
    {
        bool PreventAutoClose { get; set; }
        bool AutoClose { get; set; }
    }
}
