using LionFire.Serialization;

namespace LionFire.Serialization
{
    public class SerializationOptions
    {
        public SerializationFlags SerializationFlags { get; set; } = SerializationFlags.HumanReadable | SerializationFlags.Text;
    }
}
