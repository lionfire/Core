using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.StateMachines.Tests.NetFramework
{
    [DuplicateWithSuffix("A")]
    public class Foo
    {

        public static void Test()
        {
            var orig = new Foo();
            var x = new FooA();
        }
        
    }
}
