using LionFire.IO;
using System;
using System.IO;
using Xunit;

namespace RTextFile_;

public class UnitTest1
{
    private const string testText = "test text";

    [Fact]
    public void Retrieve()
    {
        //var path = ; // TODO: Get temp file like in FSOBus tests
        //File.WriteAllText(path, testText);

        var r = new RTextFile();
        var result = r.Value;

        Assert.Equal(testText, result);
    }
}
