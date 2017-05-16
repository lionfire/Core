using LionFire.Serialization.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Serialization
{


    public interface ISerializer
    {

        SerializationFlags SupportedCapabilities { get; }

        IEnumerable<string> FileExtensions { get; }
        IEnumerable<string> MimeTypes { get; }

        object DefaultDeserializationSettings { get; set; }
        object DefaultSerializationSettings { get; set; }

        byte[] ToBytes(object obj, object settings = null);
        string ToString(object obj, object settings = null);

        
        T ToObject<T>(byte[] serializedData, SerializationContext context = null);
        T ToObject<T>(string serializedData, SerializationContext context = null);
        
    }
}
