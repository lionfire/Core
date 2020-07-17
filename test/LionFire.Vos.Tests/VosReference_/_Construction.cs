using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using Xunit;
using LionFire.Hosting;
using LionFire.Services;
using LionFire.Dependencies;
using LionFire.Persistence.Filesystem;
using System.IO;
using LionFire.Persistence.Filesystem.Tests;
using LionFire.Vos;
using LionFire.Referencing;

namespace VobReference_
{
    public class _Construction
    {
        [Fact]
        public void P_Chunks()
        {
            var name = "testfile.subext.txt";
            var reference1 = new VobReference("testDir", "testSubdir", name);
            Assert.Equal("/testDir/testSubdir/testfile.subext.txt", reference1.Path);
        }

        [Fact]
        public void P_Relative()
        {
            var reference4 = new VobReference("testDir/testSubdir/testfile.subext.txt");
            Assert.Equal("testDir/testSubdir/testfile.subext.txt", reference4.Path);
        }


        [Fact]
        public void P_Absolute()
        {
            var reference3 = new VobReference("/testDir/testSubdir/testfile.subext.txt");
            Assert.Equal("/testDir/testSubdir/testfile.subext.txt", reference3.Path);
        }
    }
}
