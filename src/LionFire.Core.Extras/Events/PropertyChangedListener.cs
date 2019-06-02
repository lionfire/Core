namespace LionFire.Events
{

    public class PropertyChangedListener : PropertyChangedListenerBase<IPropertyChanged>, IPropertyChangedListener
    {
        protected override void Attach() => NotifyingTarget.PropertyValueChanged += NotifyingTarget_PropertyValueChanged;

        protected override void Detach() => NotifyingTarget.PropertyValueChanged += NotifyingTarget_PropertyValueChanged;

        private void NotifyingTarget_PropertyValueChanged(string obj) => OnPropertyNameChanged(obj);

        public bool HasFastChangedTo => false;

        public bool HasChangedFrom => false;

    }
}
