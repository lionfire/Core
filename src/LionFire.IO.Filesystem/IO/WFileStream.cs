using System;
using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{
    // REVIEW
    public class WFileStream : WLocalFileBase<Stream>, IDisposable
    {
        public override Task<bool> TryRetrieveObject()
        {
            var stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            base.OnRetrievedObject(stream);
            return Task.FromResult(true);
        }

        #region Construction

        public WFileStream() { }
        public WFileStream(string path) : base(path)
        {
        }

        #endregion

        /// <summary>
        /// Flushes the stream.  (Note: this may be largely irrelevant.)
        /// </summary>
        /// <param name="persistenceContext"></param>
        /// <returns></returns>
        public override async Task WriteObject(object persistenceContext = null)
        {
            await Object.FlushAsync();
            //throw new NotSupportedException("TODO: Implement if it make sense, or change this message.  You already have a Stream that you can write to yourself.");
            //    .WriteAllBytes(Path, Object);
            //    return Task.CompletedTask;
        }

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
