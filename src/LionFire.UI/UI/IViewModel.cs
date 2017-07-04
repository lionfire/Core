
namespace LionFire.UI
{
    public interface IViewModel { object Model { get; set; } }

    public interface IViewModel<T> : IViewModel
    {
        new T Model { get; set; }
        bool IsViewModelOf(object obj);
    }
}
