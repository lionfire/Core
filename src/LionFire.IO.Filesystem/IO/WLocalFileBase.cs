using System.IO;
using System.Threading.Tasks;
using LionFire.Referencing;

namespace LionFire.IO
{
    public abstract class WLocalFileBase<T> : WBase<T>
        where T : class
    {
        #region Path

        [SetOnce]
        public string Path
        {
            get { return path; }
            set
            {
                if (path == value)
                {
                    return;
                }

                if (path != default(string))
                {
                    throw new AlreadySetException();
                }

                path = value;
            }
        }
        private string path;

        #endregion

        #region Construction

        public WLocalFileBase() { }
        public WLocalFileBase(string path)
        {
            this.Path = path;
        }

        #endregion

        protected override async Task<bool?> DeleteObject(object persistenceContext = null)
        {
            return await Task.Run(() =>
            {
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                    return (bool?)true;
                }
                return null;
            }).ConfigureAwait(false);
        }

    }
}
