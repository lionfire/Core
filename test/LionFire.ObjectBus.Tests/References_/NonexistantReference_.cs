using System;
using LionFire.Referencing;
using Xunit;

namespace LocalFileReference_
{
    public class NonexistantReference_
    {
        private static readonly string bogusSchemeUri = @"bogus31415926535:///c:\Temp\Path\Test\" + Guid.NewGuid().ToString();

        [Fact]
        public void Fail_Throws() => Assert.Throws<ArgumentException>(() => bogusSchemeUri.ToReference());

        [Fact]
        public void Fail_Null() => Assert.Null(bogusSchemeUri.TryToReference());
    }
}
