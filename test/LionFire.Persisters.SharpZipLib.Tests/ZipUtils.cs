using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using LionFire.Referencing;
using System.Diagnostics;
using System.Text;
using System.Text.Unicode;

namespace LionFire.Persisters.SharpZipLib.Tests.Deserialize;

public class ZipUtils
{
    public static void ValidateZip(ZipFile zipFile)
    {
        var entries = zipFile.OfType<ZipEntry>();

        Assert.AreEqual(3, entries.Count());
        //Assert.AreEqual(1, entries["TestTargetDir"]);

        var dirEntry = zipFile[zipFile.FindEntry("TestTargetDir/", false)];
        Assert.IsNotNull(dirEntry);
        Assert.IsTrue(dirEntry.IsDirectory);

        var fileEntry = zipFile[zipFile.FindEntry("TestTargetDir/TestTargetFile.txt", false)];
        Assert.IsNotNull(fileEntry);
        Assert.IsTrue(fileEntry.IsFile);

        var z = new FastZip();

        foreach (var entry in entries)
        {
            Debug.WriteLine(entry.Name);

            if (entry.IsFile)
            {
                var buffer = new byte[4096];

                MemoryStream output = new();
                // Unzip file in buffered chunks. This is just as fast as unpacking
                // to a buffer the full size of the file, but does not waste memory.
                // The "using" will close the stream even if an exception occurs.
                using var zipStream = zipFile.GetInputStream(entry);
                StreamUtils.Copy(zipStream, output, buffer);
                var str = Encoding.UTF8.GetString(output.GetBuffer(), 0, (int)output.Length);
                Debug.WriteLine(str);
            }
        }

    }

}
