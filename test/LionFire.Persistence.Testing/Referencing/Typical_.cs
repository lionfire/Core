using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LionPath_
{
    public class Typical_
    {
        [Fact]
        public void P_Relative()
        {
            var path  = LionPath.Combine("testDir", "testSubdir", "testfile.subext.txt");
            Assert.Equal("testDir/testSubdir/testfile.subext.txt", path);
        }

        [Fact]
        public void P_Absolute()
        {
            var path = LionPath.Combine("/testDir", "testSubdir", "testfile.subext.txt");
            Assert.Equal("/testDir/testSubdir/testfile.subext.txt", path);
        }

        [Fact]
        public void P_Absolute_EndSeparator()
        {
            var path = LionPath.Combine("/testDir", "testSubdir", "testsubsubdir/");
            Assert.Equal("/testDir/testSubdir/testsubsubdir/", path);
        }
    }
}
