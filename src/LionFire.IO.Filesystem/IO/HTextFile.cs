using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{

    public class HTextFile : HLocalFileBase<string>
    {
        #region Construction

        public HTextFile() { }
        public HTextFile(string path) : base(path)
        {
        }

        #endregion

        protected override async Task WriteObject(object persistenceContext = null)
        {
            await Task.Run(() =>
            {
                File.WriteAllText(Path, Object);
            }).ConfigureAwait(false);
        }

        protected override async Task DeleteObject(object persistenceContext = null)
        {
            await Task.Run(() =>
            {
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                }
            }).ConfigureAwait(false);
        }

        public override async Task<bool> TryRetrieveObject()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(Path))
                {
                    return false;
                }
                OnRetrievedObject(File.ReadAllText(Path));
                return true;
            }).ConfigureAwait(false);
        }


    }
}
