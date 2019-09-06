using LionFire.Persistence;
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
        
        public override async Task<IRetrieveResult<string>> RetrieveImpl()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(Path))
                {
                    return RetrieveResult<string>.NotFound;
                }
                
                return RetrieveResult<string>.Success(OnRetrievedObject(File.ReadAllText(Path)));
            }).ConfigureAwait(false);
        }
    }
}
