using LionFire.Persistence;
using LionFire.Data.Gets;
using MorseCode.ITask;
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
        
        protected override async ITask<IGetResult<string>> GetImpl(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(Path))
                {
                    return RetrieveResult<string>.NotFound;
                }
                
                return RetrieveResult<string>.Success(File.ReadAllText(Path));
            }).ConfigureAwait(false);
        }
    }
}
