﻿using System.IO;
using System.Threading.Tasks;
using LionFire.Persistence;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Handles;
using LionFire.Referencing;

namespace LionFire.IO
{
    public abstract class WLocalFileBase<T> : ReadWriteHandleBase<FileReference<T>, T>
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
        public WLocalFileBase(string path, T initialData = default)
        {
            this.Path = path;
            SetValueFromConstructor(initialData);
        }

        #endregion

        protected override async Task<ITransferResult> DeleteImpl()
        {
            return await Task.Run(() =>
            {
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                    return TransferResult.Success;
                }
                return TransferResult.NotFound;
            }).ConfigureAwait(false);
        }

    }
}
