namespace LionFire.Structures;


// Also consider language-ext's Option<T>
// TODO: Rename this to IReadWrapper, and make a separate INotNullReadWrapper?
public interface IDefaultableReadWrapper<out T>: IReadWrapper<T>, IDefaultable { }
