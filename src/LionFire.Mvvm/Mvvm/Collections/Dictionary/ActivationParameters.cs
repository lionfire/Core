#nullable enable


namespace LionFire.Mvvm;

public readonly struct ActivationParameters
{
    public ActivationParameters(Type type, object[]? parameters = null)
    {
        Type = type;
        Parameters = parameters;
    }

    public Type Type { get; }
    public object[]? Parameters { get; }

    public static implicit operator ActivationParameters(Type type) => new ActivationParameters(type);
    
}
