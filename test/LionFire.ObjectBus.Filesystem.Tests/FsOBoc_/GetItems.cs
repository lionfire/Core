using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LionFire.ObjectBus.Filesystem;
using Xunit;
using System.IO;
using LionFire.Hosting;
using LionFire.ObjectBus;
using System.Threading.Tasks;

namespace FsOBoc_
{

    public class GetItems
    {
        [Fact]
        public async Task Pass()
        {
            Assert.True(true);
            await FrameworkHost.Create()
                .AddObjectBus<FsOBus>()
                .Run(() =>
                {

                    #region Constants

                    string path = null;
                    while (path == null || Directory.Exists(path)) path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    var list = new List<string>
                    {
                        "123.txt",
                        "asdf.dat",
                        "A longer filename (with spaces) [and parentheses].json"
                    };

                    Debug.WriteLine($"{this.GetType().FullName} - dir: {path}");

                    #endregion

                    #region Create test files

                    Directory.CreateDirectory(path);

                    Assert.True(Directory.Exists(path));

                    foreach (var file in list)
                    {
                        File.WriteAllText(Path.Combine(path, file), "test contents");
                    }

                    #endregion

                    var reference = new LocalFileReference(path);
                    var oboc = new FsOBoc(reference);
                    var obj = oboc.Object;

                    Assert.IsType<FsList>(obj);
                    var fsList = (FsList)obj;

                    Assert.NotEmpty(fsList); // lazily load

                    var remaining = new List<string>(list);

                    foreach (var item in fsList)
                    {
                        Debug.WriteLine(" - " + item.Name);
                        Assert.True(remaining.Remove(item.Name), "Found an unexpected file: " + item.Name);
                    }

                    Assert.Empty(remaining);

                    Directory.Delete(path, true);
                    Assert.False(Directory.Exists(path));
                });
        }
    }
}
