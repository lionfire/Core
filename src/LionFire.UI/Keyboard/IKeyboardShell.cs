using System.ComponentModel;

namespace LionFire.Inputs
{
    public interface IKeyboard :INotifyPropertyChanged
    {
        #region State

        /// <summary>
        /// Set to true by applications when keyboard is receiving text input.  This can be useful in determining whether certain keyboard events and hotkeys should be disabled while the user is typing.  When the user is typing, it may be desirable to disable hotkeys (especially ones that use letters without modifier keys.)
        /// </summary>
        bool IsEditingText { get; set; }

        #endregion
    }

    public class Keyboard : IKeyboard
    {

        #region IsEditingText

        public bool IsEditingText
        {
            get => isEditingText;
            set
            {
                if (isEditingText == value) return;
                isEditingText = value;
                OnPropertyChanged(nameof(IsEditingText));
            }
        }
        private bool isEditingText;

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

    }
}
