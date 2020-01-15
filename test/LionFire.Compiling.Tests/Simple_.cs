using LionFire.Compiling;
using System;
using Xunit;

namespace Compiling_
{
    public class Simple_
    {
        [Fact]
        public void Pass()
        {
            var code = @"    public class Greeter
    {
        public string Hello(int iteration)
        {
            var msg = $""Hello {iteration}!"";
            return msg;
        }
}
";
            var code2 = @"using System;

namespace SampleLibrary
{
    public class Greeter
    {
        public string Hello(int iteration)
        {
            var msg = $""Hello {iteration}!"";
            return msg;
        }
    }
}
";

            var options = new CompilerOptions();
            var c = new Compiler();

            {
                var compilation = c.Compile(code, options);
                var result = c.Run(compilation, "Greeter", "Hello", parameters: new object[] { 456 });
                Assert.Equal("Hello 456!", result.ReturnValue);
            }
            {
                var compilation = c.Compile(code2, options);
                var result = c.Run(compilation, "SampleLibrary.Greeter", "Hello", parameters: new object[] { 123 });
                Assert.Equal("Hello 123!", result.ReturnValue);
            }
            // FUTURE ENH: Ensure No memory leaks
        }
    }
}
