using LionFire.Structures;
using System.ComponentModel;

namespace LionFire.Data.Id;

public class IdedSerializable<T> : SerializableIdentifiableBase<T>, INotifyPropertyChanged, IDisplayNamable
{
    #region Construction

    public IdedSerializable() { }
    public IdedSerializable(string id) : base(id) { }

    #endregion

    #region DisplayNamable

    public string DisplayName
    {
        get => displayName ?? DefaultDisplayName;
        set => displayName = value;
    }
    private string displayName;

    public virtual string DefaultDisplayName => null;

    #endregion

    #region INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    #endregion
}
