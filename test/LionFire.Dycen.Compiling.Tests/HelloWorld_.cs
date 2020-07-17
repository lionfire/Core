using LionFire.Compiling;
using LionFire.Hosting;
using LionFire.Referencing;
using LionFire.Vos;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Compilation_
{
    public class HelloWorld_
    {
#if TODO
        [Fact]
        public void Pass()
        {
            VosHost.Create()
                .Run(serviceProvider =>
                {

                    var root = serviceProvider.GetRequiredService<VosRootManager>().Get();

                    var rwH = "/TestScript".ToVobReference().ToReadWriteHandle<string>();
                    Assert.NotNull(rwH); 

                    var code = @"using System;

namespace SampleLibrary
{
    public class Greeter
    {
        public string Hello(int iteration)
        {
            var msg = $""Hello {iteration}!"";
            Debug.WriteLine(msg);
            return msg;
        }
    }
    }
";

                    var options = root["TestScript"].GetNext<CompilerOptions>();

                    var c = new Compiler();
                    var compilation = c.Compile(rwH.Value, options);

                    var result = c.Run(compilation, "SampleLibrary.Greeter", "Hello", parameters: new object[] { 123 });

                    Assert.Equal("Hello 123!", result.ReturnValue);

                    Assert.True(true);

                });
        }
#endif
    }
}
