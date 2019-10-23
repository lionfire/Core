using LionFire.Persistence;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{
    // REVIEW
    public class RFileStream : RLocalFileBase<Stream>, IDisposable
    {
        public override Task<IRetrieveResult<Stream>> RetrieveImpl()
        {
            var stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult((IRetrieveResult<Stream>)RetrieveResult<Stream>.Success(OnRetrievedObject(stream)));
        }

        #region Construction

        public RFileStream() { }
        public RFileStream(string path, Stream initialData = default) : base(path)
        {
        }

        #endregion

        public void Dispose()
        {
            var obj = base._object;
            if (_object != null)
            {
                DiscardValue();
                _object.Dispose();
            }
        }
    }
}
