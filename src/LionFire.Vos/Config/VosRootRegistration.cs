

namespace LionFire.Vos
{
    public class VosRootRegistration
    {
        public string Name { get; set; }

        public VosRootRegistration(string name = null) { Name = name; }

        public static implicit operator VosRootRegistration(string name) => new VosRootRegistration(name);
    }
}
