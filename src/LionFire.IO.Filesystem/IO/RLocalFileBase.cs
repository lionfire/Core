using LionFire.Persistence;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Handles;
using LionFire.Referencing;

namespace LionFire.IO
{
    public abstract class RLocalFileBase<T> : ReadHandleBase<FileReference, T>
        where T : class
    {
        #region Path

        [SetOnce]
        public string Path
        {
            get => Reference?.Path;
            set { Reference = value; }
        }

        #endregion

        #region Construction

        public RLocalFileBase() { }
        public RLocalFileBase(string path)
        {
            this.Path = path;
        }

        #endregion

    }
}
