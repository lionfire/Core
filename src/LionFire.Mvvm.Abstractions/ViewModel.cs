using LionFire.Structures;

namespace LionFire.Mvvm;

// Default implementation of a IViewModel -- useless? Or good base class?
public class ViewModel<T> : IViewModel<T>, IReadWrapper<T>
{
    public T? Model { get; set; }
    object? IViewModel.Model { get { return Model; } set { Model = (T?)value; } }

    T? IReadWrapper<T>.Value => Model;
}
