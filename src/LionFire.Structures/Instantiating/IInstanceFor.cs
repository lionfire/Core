namespace LionFire.Instantiating
{
    public interface IInstanceFor<T>  // RENAME? InstanceOf?
    {
         T Template { get; set; } // RENAME? InstanceSource?  Source?
    }
}
