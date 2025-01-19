namespace LionFire.Orleans_.Reactive_;

public interface IAsyncObservableO<TValue> 
    : System.IAsyncObserver<TValue> // ENH: [OneWay] on IAsyncObserver methods
    , IGrainObserver
{

}
