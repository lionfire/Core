using LionFire.Serialization.Contexts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Serialization
{
    public interface ISerializer
    {
        byte[] ToBytes(SerializationContext context);
        string ToString(SerializationContext context);

        T ToObject<T>(SerializationContext context);
    }
}
