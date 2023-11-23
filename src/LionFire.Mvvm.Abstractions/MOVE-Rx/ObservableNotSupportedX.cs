
namespace LionFire.Mvvm;

public static class ObservableNotSupportedX // MOVE
{
    private class NotSupportedDetector<T> : IObserver<T>
    {
        public bool IsSupported { get; private set; } = true;

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            if (error is NotSupportedException) IsSupported = false;
        }

        public void OnNext(T value)
        {
        }
    }

    public static bool IsSupported<TItem>(this IObservable<TItem> o)
    {
        var detector = new NotSupportedDetector<TItem>();
        using var subscription = o.Subscribe(detector);
        return detector.IsSupported;
    }
}

