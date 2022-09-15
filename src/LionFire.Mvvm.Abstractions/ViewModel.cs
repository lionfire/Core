using LionFire.Structures;

namespace LionFire.Mvvm;

// Default implementation of a IViewModel -- useless? Or good base class?
public class ViewModel<T> : IViewModel<T>
{
    public T? Value
    {
        get => value;
        set
        {
            if (ReferenceEquals(value, this.value)) return;
            if(value != null) { throw new AlreadySetException(); }
            this.value = value;
        }
    }
    private T? value;
    //T? IReadWrapper<T>.Value => Value;
    
    //object? IViewModel.Model { get { return Value; } set { Value = (T?)value; } }

}
