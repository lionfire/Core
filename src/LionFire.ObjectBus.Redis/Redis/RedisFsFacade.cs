using LionFire.Persistence;
using LionFire.Persistence.FilesystemFacade;
using LionFire.Referencing;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.Redis
{

    public class RedisFsFacade : ISimpleFilesystemFacade
    {
        #region Relationships

        private RedisOBase obase;

        #endregion

        #region Convenience

        private IDatabase Db => obase.Redis.GetDatabase();

        #endregion

        #region Construction

        public RedisFsFacade(RedisOBase obase)
        {
            this.obase = obase;
        }

        #endregion

        public static (string dir, string name) BreakPath(string path) => (LionPath.GetDirectoryName(path), LionPath.GetFileName(path));

        public async Task<bool> Exists(string path) => await Db.KeyExistsAsync(path).ConfigureAwait(false);
        public async Task<IPersistenceResult> Delete(string path)
        {
            var (dir, name) = BreakPath(path);
            bool deleted = await Db.SetRemoveAsync(dir + "/", name).ConfigureAwait(false);
            await Db.KeyDeleteAsync(path).ConfigureAwait(false);
            return deleted ? PersistenceResult.Success : PersistenceResult.NotFound;
        }

        public async Task<byte[]> ReadAllBytes(string path) => await Db.StringGetAsync(path).ConfigureAwait(false);
        public async Task<string> ReadAllText(string path) => await Db.StringGetAsync(path).ConfigureAwait(false);

        public async Task WriteAllBytes(string path, byte[] data) => await Db.StringSetAsync(path, data).ConfigureAwait(false);
        public async Task WriteAllText(string path, string data) => await Db.StringSetAsync(path, data).ConfigureAwait(false);

        #region GetFiles

        public static int GetFilesPageSize = 1000;

        // Non-blocking
        public async Task<IEnumerable<string>> List(string directoryPath, string pattern = null)
        {
            return await Task.Run(() =>
            {
                var result = Db.SetScan(directoryPath, pattern, GetFilesPageSize);
                //IScanningCursor cur = (IScanningCursor)result;
                //cur.

                return result.ToArray().Select(rv => (string)rv);
            }).ConfigureAwait(false);
        }

        // Blocking on get next page (?)
        public async Task<IEnumerable<string>> GetFilesBlocking(string directoryPath, string pattern = null)
        {
            return await Task.Run(() =>
            {
                if (!directoryPath.EndsWith("/"))
                {
                    directoryPath += "/";
                }

                var result = Db.SetScan(directoryPath, pattern, GetFilesPageSize);
                //IScanningCursor cur = (IScanningCursor)result;
                //cur.

                return result.Select(rv => (string)rv);
            }).ConfigureAwait(false);
        }

        #endregion

    }



}
