using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LionFire.ObjectBus.Filesystem.Tests
{
    public class LocalFileReferenceTests
    {
        [Fact]
        public void Path_to_Url()
        {
            var pathWithoutExtension = @"c:\Temp\Path\Test\" + Guid.NewGuid().ToString();

            var reference = new LocalFileReference(pathWithoutExtension);

            Assert.Equal("file:///" + pathWithoutExtension, reference.Key);
        }
    }
}
