using LionFire.Persistence;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{
    // REVIEW
    public class WFileStream : WLocalFileBase<Stream>, IDisposable
    {

        public override Task<IRetrieveResult<Stream>> RetrieveImpl()
        {
            var stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult((IRetrieveResult<Stream>)RetrieveResult<Stream>.Success(OnRetrievedObject(stream)));
        }

        // OLD
        //public override Task<IRetrieveResult<Stream>> RetrieveImpl()
        //{
        //    if (stream != null) return Task.FromResult((IRetrieveResult<Stream>)RetrieveResult<Stream>.Noop(stream));

        //    stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    base.OnRetrievedObject(stream); // REVIEW - only retrieved the stream object.  User still needs to pull data.  Document this better?
        //    return Task.FromResult((IRetrieveResult<Stream>)RetrieveResult<Stream>.Success(stream));
        //}

        #region Construction

        public WFileStream() { }
        public WFileStream(string path, Stream initialData = default) : base(path,initialData)
        {
        }

        #endregion

        /// <summary>
        /// Flushes the stream.  (Note: this may be largely irrelevant.)
        /// </summary>
        /// <param name="persistenceContext"></param>
        /// <returns></returns>
        protected override async Task<IPersistenceResult> WriteObject()
        {
            if (HasValue)
            {
                await Value.FlushAsync();
                return PersistenceResult.Success;
            }
            else
            {
                throw new InvalidOperationException("Object is not set.  Cannot write.");
            }
            //throw new NotSupportedException("TODO: Implement if it make sense, or change this message.  You already have a Stream that you can write to yourself.");
            //    .WriteAllBytes(Path, Object);
            //    return Task.CompletedTask;
        }

        public void Dispose()
        {
            var obj = base._value;
            if (_value != null)
            {
                DiscardValue();
                _value.Dispose();
            }
        }
    }
}
