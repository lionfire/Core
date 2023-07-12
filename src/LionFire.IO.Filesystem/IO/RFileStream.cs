using LionFire.Persistence;
using LionFire.Data.Gets;
using MorseCode.ITask;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{
    // REVIEW
    public class RFileStream : RLocalFileBase<Stream>, IDisposable
    {
        #region Construction

        public RFileStream() { }
        public RFileStream(string path, Stream initialData = default) : base(path)
        {
        }

        #endregion

        protected override ITask<IGetResult<Stream>> GetImpl(CancellationToken cancellationToken = default)
        {
            var stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult((IGetResult<Stream>)RetrieveResult<Stream>.Success(stream)).AsITask();
        }

        public override void Dispose()
        {
            var obj = base.ReadCacheValue;
            if (obj != null)
            {
                DiscardValue();
                obj.Dispose();
            }

            base.Dispose();
        }
    }
}
