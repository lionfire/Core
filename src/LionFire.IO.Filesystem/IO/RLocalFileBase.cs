using LionFire.Referencing;

namespace LionFire.IO
{
    public abstract class RLocalFileBase<T> : RBase<T>
        where T : class
    {
        #region Path

        [SetOnce]
        public string Path
        {
            get => path;
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

        public RLocalFileBase() { }
        public RLocalFileBase(string path)
        {
            this.Path = path;
        }

        #endregion

    }
}
