namespace LionFire.UI.Entities 
{
    public interface IWindow : IUIKeyed
    {
        bool Topmost { get; }
        void Restore();
        void Minimize();
        void Maximize();


    }
}
