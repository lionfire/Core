namespace LionFire.Instantiating;

public interface IInstanceForSettable<T> : IInstanceFor<T>  // RENAME? InstanceOf?
{
    new T Template { get; set; } // RENAME? InstanceSource?  Source?
}
public interface IInstanceFor<T>
{
    T Template { get; }
}
