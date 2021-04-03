using System;
using LionFire.Referencing;
using LionFire;
using LionFire.Hosting;
using Xunit;

namespace LocalFileReference_
{
    public class NonexistantReference_ : FrameworkHostBase
    {
        private static readonly string bogusSchemeUri = @"bogus31415926535:///c:\Temp\Path\Test\" + Guid.NewGuid().ToString();

        [Fact]
        public void F_Throws() => Assert.Throws<NotFoundException>(() => bogusSchemeUri.ToReference());

        [Fact]
        public void F_Null() => Assert.Null(bogusSchemeUri.TryToReference());
        
    }
}
