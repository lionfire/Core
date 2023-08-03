using LionFire.Persistence;
using LionFire.Data.Async.Gets;
using MorseCode.ITask;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{
    // REVIEW
    public class WFileStream : WLocalFileBase<Stream>, IDisposable
    {

        #region Construction

        public WFileStream() { }
        public WFileStream(string path, Stream initialData = default) : base(path,initialData)
        {
        }

        #endregion

        protected override ITask<IGetResult<Stream>> GetImpl(CancellationToken cancellationToken = default)
        {
            var stream = new FileStream(Path, FileMode.Open, FileAccess.Write, FileShare.Write);
            return Task.FromResult((IGetResult<Stream>)RetrieveResult<Stream>.Success(stream)).AsITask();
        }

        // OLD
        //public override Task<IGetResult<Stream>> RetrieveImpl()
        //{
        //    if (stream != null) return Task.FromResult((IGetResult<Stream>)RetrieveResult<Stream>.Noop(stream));

        //    stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    base.OnRetrievedObject(stream); // REVIEW - only retrieved the stream object.  User still needs to pull data.  Document this better?
        //    return Task.FromResult((IGetResult<Stream>)RetrieveResult<Stream>.Success(stream));
        //}

        /// <summary>
        /// Flushes the stream.  (Note: this may be largely irrelevant.)
        /// </summary>
        /// <param name="persistenceContext"></param>
        /// <returns></returns>
        protected override async Task<ITransferResult> UpsertImpl()
        {
            if (HasValue)
            {
                await Value.FlushAsync();
                return TransferResult.Success;
            }
            else
            {
                throw new InvalidOperationException("Object is not set.  Cannot write.");
            }
            //throw new NotSupportedException("TODO: Implement if it make sense, or change this message.  You already have a Stream that you can write to yourself.");
            //    .WriteAllBytes(Path, Object);
            //    return Task.CompletedTask;
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
