namespace LionFire.UI
{
    public interface IPresenterContainer
    {
        System.Collections.Generic.IReadOnlyDictionary<string, IPresenter> Presenters { get; }
    }
}
