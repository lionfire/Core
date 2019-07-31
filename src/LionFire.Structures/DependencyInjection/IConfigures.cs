
namespace LionFire
{
    /*  public interface IAffects<T>
      {
          void Affect(T target);
      }
  */
    public interface IConfigures { }
    public interface IConfigures<T> : IConfigures
        where T : class
    {
        void Configure(T context); // TODO: Return this for fluent interfaces?
    }
}
