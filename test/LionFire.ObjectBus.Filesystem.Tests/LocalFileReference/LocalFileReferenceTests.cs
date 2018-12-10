using System;
using System.Collections.Generic;
using System.Text;
using LionFire.ObjectBus.Filesystem;
using Xunit;

namespace LocalFileReference_
{
    public class Path_to_Url
    {
        [Fact]
        public void Pass()
        {
            var pathWithoutExtension = @"c:\Temp\Path\Test\" + Guid.NewGuid().ToString();

            var reference = new LocalFileReference(pathWithoutExtension);

            Assert.Equal("file:///" + pathWithoutExtension, reference.Key);
        }
    }
}
