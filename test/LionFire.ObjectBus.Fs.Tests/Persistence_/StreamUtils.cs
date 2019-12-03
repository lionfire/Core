//using LionFire.ObjectBus.Filesystem.Persisters;
using System;
using Xunit;
using System.IO;
using LionFire.Serialization;

namespace Persistence_
{
    public class StreamUtils
    {
        [Fact]
        public void P_StreamToBytes()
        {
            var testContents = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 32, 33, 34, 35, 64, 65, 66, 67, 68 };

            var ms = new MemoryStream();
            ms.Write(new ReadOnlySpan<byte>(testContents));
            ms.Seek(0, SeekOrigin.Begin);

            var result = ms.StreamToBytes();

            Assert.Equal(testContents, result);
        }
    }
}