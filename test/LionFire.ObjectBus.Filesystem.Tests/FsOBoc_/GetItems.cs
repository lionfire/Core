using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LionFire.ObjectBus.Filesystem;
using Xunit;

namespace FsOBoc_
{

    public class GetItems
    {
        [Fact]
        public void Pass()
        {

            var path = @"d:\temp"; // TEMP HARDCODE FIXME

            var reference = new LocalFileReference(path);

            var oboc = new FsOBoc(reference);

            var obj = oboc.Object;

            Assert.IsType<FsList>(obj);
            var fsList = (FsList)obj;

            Assert.NotEmpty(fsList);

            foreach(var item in fsList)
            {
                Debug.WriteLine(" - " + item.Name);
            }

        }
    }
}
