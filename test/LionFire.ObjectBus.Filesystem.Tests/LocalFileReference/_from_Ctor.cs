using System;
using System.Collections.Generic;
using System.Text;
using LionFire.ObjectBus.Filesystem;
using LionFire.Referencing;
using Xunit;

namespace LocalFileReference_
{
    public class _from_Ctor
    {
        [Fact]
        public void Pass()
        {
            var pathWithoutExtension = @"c:\Temp\Path\Test\" + Guid.NewGuid().ToString();

            var reference = new LocalFileReference(pathWithoutExtension);

            Assert.Equal("file:///" + pathWithoutExtension.Replace('\\', '/'), reference.Key);
        }
    }
}
