using LionFire.Persistence.Handles;
using LionFire.Referencing;
using System;
using System.Threading.Tasks;

namespace LionFire.IO
{

    public abstract class HLocalFileBase<T> : ReadWriteHandleBaseEx<T>
        where T : class
    {
        //public override string Key { get => Path; set => Path = Key; }

        //public override IReference Reference
        //{
        //    get
        //    {
        //        // TODO: Populate a File or UrlReference with hostname?
        //        // e.g. file:///c/Temp/file.txt - no host specified, assume using localhost wherever this URL shows up
        //        // e.g. file://localhost/c/Temp/file.txt - if localhost explicitly specified
        //        // e.g. file://mymachine/c/Temp/file.txt - Explicit host
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        // TODO: Make sure machine name matches?  If this class only deals with local files
        //        throw new NotImplementedException();
        //    }
        //    //Path = ((FileReference)value).Path;
        //}

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

                if (path != default)
                {
                    throw new AlreadySetException();
                }

                path = value;
            }
        }
        private string path;

        #endregion

        #region Construction

        public HLocalFileBase() { }
        public HLocalFileBase(string path, T initialObject = default)
        {
            this.Path = path;
            SetValueFromConstructor(initialObject);
        }
        
        #endregion

    }
}
