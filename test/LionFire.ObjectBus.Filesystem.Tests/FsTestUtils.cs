using System;
using System.IO;
using System.Runtime.CompilerServices;
using LionFire.ObjectBus.Filesystem.Tests;
using Xunit;

namespace LionFire.ObjectBus.Filesystem.Tests
{
    public class FsTestUtils
    {
        public static bool EnableFileCleanup = true;

        

        public static string DataDir => Path.GetTempPath();

        public static void AssertEqual(TestClass1 obj, object deserialized)
        {
            Assert.Equal(typeof(TestClass1), deserialized.GetType());
            var obj2 = (TestClass1)deserialized;
            Assert.Equal(obj.StringProp, obj2.StringProp);
            Assert.Equal(obj.IntProp, obj2.IntProp);
            Assert.Equal(obj.Object.StringProp2, obj2.Object.StringProp2);
            Assert.Equal(obj.Object.IntProp2, obj2.Object.IntProp2);
        }

        public static string TestFile => Path.Combine(DataDir, "UnitTest " + Guid.NewGuid().ToString());

        public static void CleanPath(string path)
        {
            if (!path.StartsWith(DataDir)) throw new ArgumentException("CleanPath only works for files in Path.GetTempPath()");
            if (!EnableFileCleanup) return;

            File.Delete(path);
            Assert.False(File.Exists(path), "Cleanup failed.  Delete file: " + path);
        }
    }
}
