#nullable enable
using CircularBuffer;
using System.Collections.Generic;

namespace LionFire.Diagnostics
{
    // TODO: Split into two classes
    public class CombinedProcessOutput
    {
        public IEnumerable<string>? CombinedOutputLines => CombinedOutputList ?? (IEnumerable<string>?)CombinedOutputBuffer;

        public CircularBuffer<string>? CombinedOutputBuffer { get; set; }
        public List<string>? CombinedOutputList { get; set; }

        // TODO: Event?
    }

    //public class CombinedConsoleOutput
    //{
    //    public CircularBuffer<string>? Output { get; set; }

    //    public event Action<string[]> OutputReceived;
    //}

    //public class SeparatedConsoleOutput
    //{
    //    public CircularBuffer<string>? StandardOutput { get; set; }
    //    public CircularBuffer<string>? StandardError { get; set; }
    //}

}
