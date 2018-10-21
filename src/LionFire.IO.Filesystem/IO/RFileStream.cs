using System;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{
    // REVIEW
    public class RFileStream : RLocalFileBase<Stream>, IDisposable
    {
        public override Task<bool> TryRetrieveObject()
        {
            var stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            base.OnRetrievedObject(stream);
            return Task.FromResult(true);
        }

        #region Construction

        public RFileStream() { }
        public RFileStream(string path) : base(path)
        {
        }

        #endregion

        public void Dispose()
        {
            var obj = base._object;
            if (_object != null)
            {
                ForgetObject();
                _object.Dispose();
            }
        }
    }
}
