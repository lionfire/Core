using LionFire.DependencyMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Gates_
{

    //public class Dependant1 : IReactor
    //{

    //}

    public class NamedGate_
    {
        [Fact]
        public void Pass()
        {
            var dsm = new DependencyStateMachine();

            ManualDependencyContributor alpha1, alpha2;
            dsm.Register(alpha1 = new ManualDependencyContributor("alpha"));
            dsm.Register(alpha2 = new ManualDependencyContributor("alpha"));

            alpha1.StartResetEvent.Set();
            alpha2.StartResetEvent.Set();

            Assert.Equal(2, dsm.Members.Count());

            Assert.Empty(dsm.Started);
            Assert.Equal(2, dsm.Stopped.Count());

            dsm.StartAsync().Wait();
            Assert.True(dsm.IsStarted);

            Assert.Equal(2, dsm.Started.Count());
            Assert.Empty(dsm.Stopped);

            dsm.StopAsync().Wait();
            Assert.False(dsm.IsStarted);

            Assert.Empty(dsm.Started);
            Assert.Equal(2, dsm.Stopped.Count());
        }
    }
}
