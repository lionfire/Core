using LionFire.Referencing;
using Xunit;

namespace LionPath_
{
    public class Backslash_
    {
        
        [Fact]
        public void P_Absolute()
        {
            var path = LionPath.Combine("\\testDir", "testSubdir", "testfile.subext.txt");
            Assert.Equal("/testDir/testSubdir/testfile.subext.txt", path);
        }

        [Fact]
        public void P_Absolute_EndSeparator()
        {
            var path = LionPath.Combine("/testDir", "testSubdir", "testsubsubdir\\");
            Assert.Equal("/testDir/testSubdir/testsubsubdir/", path);
        }
    }
}
