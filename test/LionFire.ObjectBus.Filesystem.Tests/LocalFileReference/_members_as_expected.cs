using System;
using LionFire.ObjectBus.Filesystem;
using Xunit;

namespace LocalFileReference_
{
    public class _members_as_expected
    {
        [Fact]
        public void Pass()
        {
            var pathWithoutExtension = @"c:\Temp\Path\Test\" + Guid.NewGuid().ToString();

            var reference = new LocalFileReference(pathWithoutExtension);

            Assert.Equal("file", reference.Scheme);
            Assert.Equal(string.Empty, reference.Host);
            Assert.Equal(string.Empty, reference.Port);
            Assert.Equal(pathWithoutExtension.Replace('\\', '/'), reference.Path);
            Assert.Equal("file:///" + pathWithoutExtension.Replace('\\', '/'), reference.Key);
        }
    }
}
