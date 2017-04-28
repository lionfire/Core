using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LionFire.Execution;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;

namespace LionFire.Applications.Tests
{
    public class TestComponent1 : IInitializable
    {
        public const string Message = "Hello from TestComponent1";

        public string GetMessage() => Message;

        public Task<bool> Initialize()
        {
            throw new NotImplementedException();
        }
        public bool IsInitialized;
    }
    public class TestComponent1a : IInitializable
    {
        public bool IsInitialized;

        [Dependency]
        public TestComponent1 Dependency { get; set; }

        public Task<bool> Initialize()
        {
            Assert.IsNotNull(Dependency);
            Assert.AreEqual(TestComponent1.Message, Dependency.GetMessage());
            return Task.FromResult(true);
        }
    }

    [TestClass]
    public class DependencyResolutionTest
    {
        [TestMethod]
        public void ResolveDependency()
        {
            var app = new AppHost();
            app
                .Add<TestComponent1a>()
                .Add<TestComponent1>()
                .Run()
                ;
        }
    }
}
