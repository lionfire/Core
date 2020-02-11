using LionFire;
using LionFire.Dependencies;
using LionFire.Hosting;
using LionFire.Persistence;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Filesystem.Tests;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace List_
{
    public class Filenames_
    {
        public static readonly string[] FileList = new string[]
        {
            "file1",
            "file2",
            ".hidden1",
            ".hidden2",
            "_special1",
            "_special2",
            "^meta1",
            "^meta2"
        };
        public static readonly string[] DirList = { "dir1", "dir2" };

        public static IEnumerable<string> ExpectedAll => FileList.Concat(ExpectedDirs);
        public static IEnumerable<string> ExpectedDefault => new string[] { "file1", "file2", "dir1/", "dir2/" };
        public static IEnumerable<string> ExpectedNormalFiles => new string[] { "file1", "file2" };
        public static IEnumerable<string> ExpectedFiles => ExpectedNormalFiles.Concat(ExpectedHidden).Concat(ExpectedMeta).Concat(ExpectedSpecial);
        public static IEnumerable<string> ExpectedDirs => new string[] { "dir1/", "dir2/" };
        public static IEnumerable<string> ExpectedHidden => new string[] { ".hidden1", ".hidden2" };
        public static IEnumerable<string> ExpectedSpecial => new string[] { "_special1", "_special2" };
        public static IEnumerable<string> ExpectedMeta => new string[] { "^meta1", "^meta2" };

        private Task P_Common(IEnumerable<string> expected, ListFilter? filter = null)
        {
            return FilesystemTestHost.Create()
                       .RunAsync(async () =>
                       {
                           var guid = Guid.NewGuid();
                           var dir = Path.Combine(FsTestUtils.DataDir, "UnitTest - " + guid.ToString());
                           var dirRef = dir.ToFileReference();

                           Assert.False(Directory.Exists(dir));
                           Directory.CreateDirectory(dir);
                           Assert.True(Directory.Exists(dir));

                           foreach (var file in FileList) { File.WriteAllText(Path.Combine(dir, file), guid.ToString() + " " + file); }
                           foreach (var dirItem in DirList) { Directory.CreateDirectory(Path.Combine(dir, dirItem)); }

                           var persister = ServiceLocator.Get<FilesystemPersister>();
                           var results = await persister.List(dirRef, filter);

                           foreach (var child in results.Value) { Debug.WriteLine($" - {child}"); }
                           Assert.Equal(expected.OrderBy(x => x), results.Value.OrderBy(x => x)); // Primary assertion

                           #region Cleanup

                           foreach (var file in FileList) { File.Delete(Path.Combine(dir, file)); }
                           foreach (var dirItem in DirList) { Directory.Delete(Path.Combine(dir, dirItem)); }
                           Directory.Delete(dir);
                           Assert.False(Directory.Exists(dir));

                           #endregion
                       });
        }

        [Fact]
        public async void P_All_via_Omit() => await P_Common(ExpectedAll);
        [Fact]
        public async void P_All_via_Null() => await P_Common(ExpectedAll, null);
        [Fact]
        public async void P_All_via_None() => await P_Common(ExpectedAll, new ListFilter { Flags = ItemFlags.None });
        [Fact]
        public async void P_All_via_All() => await P_Common(ExpectedAll, new ListFilter { Flags = ItemFlags.All });
        [Fact]
        public async void P_Files() => await P_Common(ExpectedFiles, new ListFilter { Flags = ItemFlags.File });
        [Fact]
        public async void P_Dirs() => await P_Common(ExpectedDirs, new ListFilter { Flags = ItemFlags.Directory });
        //[Fact]
        //public async void P_Hidden() => await P_Common(ExpectedHidden, new ListFilter { Flags = ItemFlags.Hidden });
        //[Fact]
        //public async void P_Meta() => await P_Common(ExpectedMeta, new ListFilter { Flags = ItemFlags.Meta });
        //[Fact]
        //public async void P_Special() => await P_Common(ExpectedSpecial, new ListFilter { Flags = ItemFlags.Special });
    }

    // FUTURE if 
    //public static class VobListExtensions
    //{
    //    public static IRet ListMeta()
    //}
}
