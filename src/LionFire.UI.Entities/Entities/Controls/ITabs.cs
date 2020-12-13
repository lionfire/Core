namespace LionFire.UI.Entities
{
    public interface ITabs : IUICollection
    {

    }

    public class Tabs : Presenter<ITab>
    {
        public override void Show(string key)
        {
            base.Show(key);
        }
        public override void Hide(string key)
        {
            base.Hide(key);
        }
    }
}