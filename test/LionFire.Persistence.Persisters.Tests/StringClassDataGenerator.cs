//using LionFire.ObjectBus.Filesystem.Persisters;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Universal_
{
    public class StringClassDataGenerator : IEnumerable<object[]>
    {
        public StringClassDataGenerator(params string[] values)
        {
            _data = new List<object[]>(values.Select(v => new object[] { v }));
        }
        private readonly List<object[]> _data;

        //public IEnumerator<string> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
        IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator() => _data.GetEnumerator();
    }
}