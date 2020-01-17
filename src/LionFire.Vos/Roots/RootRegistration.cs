

namespace LionFire.Vos
{
    public class RootRegistration
    {
        public string Name { get; set; }

        public RootRegistration(string name = null) { Name = name; }

        public static implicit operator RootRegistration(string name) => new RootRegistration(name);
    }
}
