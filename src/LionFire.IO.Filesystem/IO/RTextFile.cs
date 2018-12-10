using System.IO;
using System.Threading.Tasks;

namespace LionFire.IO
{
    public class RTextFile : RLocalFileBase<string>
    {
        #region Construction

        public RTextFile() { }
        public RTextFile(string path) : base(path)
        {
        }

        #endregion
        
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
