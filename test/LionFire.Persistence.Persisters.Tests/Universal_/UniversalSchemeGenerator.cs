//using LionFire.ObjectBus.Filesystem.Persisters;

using LionFire;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Universal_
{
    //public class UniversalSchemeGenerator : StringClassDataGenerator
    //{
    //    public UniversalSchemeGenerator() : base("couch", "file") { }

    //}

    public class UniversalPersistersGenerator : IEnumerable<object[]>
    {
        public UniversalPersistersGenerator()
        {
            _data = new List<object[]>(Objects.Select(o => new object[] { o }));
        }

        public IEnumerable<object> Objects
        {
            get
            {
                //yield return new CouchDBTests(); // Do any work yet?
                yield return new FileTests();
            }
        }
        private readonly List<object[]> _data;

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
        IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator() => _data.GetEnumerator();
        
        #endregion
    }
}