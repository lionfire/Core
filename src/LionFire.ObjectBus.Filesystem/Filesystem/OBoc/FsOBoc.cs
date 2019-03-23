using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.Filesystem
{
    public class FsOBoc : FsOBoc<object> {

        #region Construction

        public FsOBoc() { }
        public FsOBoc(LocalFileReference reference) : base(reference)
        {
        }

        #endregion

    }

    public class FsOBoc<TObject> : SyncableOBoc<TObject, FsListEntry>

    {

        #region Construction

        public FsOBoc() { }
        public FsOBoc(LocalFileReference reference) : base(reference)
        {
        }

        #endregion

        public override bool IsReadSyncEnabled { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override IEnumerator<TObject> GetEnumerator() => throw new System.NotImplementedException();

        public override async Task<bool> TryRetrieveObject()
        {
            var dir = Reference.Path;
            return await Task.Run(async () =>
           {
               if (!Directory.Exists(dir))
               {
                   if (HasObject)
                   {
                       var fsList = (FsList)Object;
                       fsList.OnDirectoryDoesNotExist();
                   }
                   return false;
               }


               if (HasObject)
               {
                   var fsList = (FsList)Object;
                   await fsList.Refresh();
                   OnRetrievedObjectInPlace();
               }
               else
               {
                   var obj = new FsList(dir);
                   await obj.Refresh();
                   OnRetrievedObject(obj);
               }

               return true;
           });
        }
    }
}
