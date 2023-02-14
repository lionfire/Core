using CircularBuffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Buffers;

public interface IBufferReceiver
{

}

public class LFCircularBuffer<T>
{
    public CircularBuffer<T> Items { get; } = new CircularBuffer<T>(400);
}

//public class BufferReceiver : I
//{
//}
