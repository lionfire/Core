//using LionFire.ObjectBus.Filesystem.Persisters;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UniversalPersister_
{
    public class UniversalSchemeGenerator : StringClassDataGenerator
    {
        public UniversalSchemeGenerator() : base("couch", "file") { }

    }

    public class UniversalPersistersGenerator : IEnumerable<object[]>
    {
        public IEnumerable<object> Objects
        {
            get
            {
                yield return new CouchDBTests();
                yield return new FileTests();
            }
        }
        public UniversalPersistersGenerator()
        {
            _data = new List<object[]>(Objects.Select(o => new object[] { o }));
        }

        private readonly List<object[]> _data;

        //public IEnumerator<string> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
        IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator() => _data.GetEnumerator();
    }
}