

namespace LionFire.Vos
{
    public interface IRootRegistration
    {
        string Name { get; }
    }
    public class RootRegistration : IRootRegistration
    {
        public string Name { get; set; }

        public RootRegistration(string name = null) { Name = name; }

        public static implicit operator RootRegistration(string name) => new RootRegistration(name);
    }
}
