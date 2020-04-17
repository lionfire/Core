using LionFire.DependencyMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace OS_
{
    public class Example_
    {
        [Fact]
        public void Pass()
        {
            var dsm = new DependencyStateMachine();


            dsm.Register(new DependencyProvider("kernel"));
            dsm.Register(new DependencyProvider("hardwareLayer", "kernel"));
            dsm.Register(new DependencyContributor("networkDrivers", "eth0", "hardwareLayer"));
            dsm.Register(new DependencyContributor("networkDrivers", "eth1", "hardwareLayer"));
            dsm.Register(new DependencyContributor("networkDrivers", "eth2", "hardwareLayer"));
            dsm.Register(new DependencyContributor("networkDrivers", null, "hardwareLayer")); // Key: networkDrivers-(D4FB2F18-67B8-46C2-BF11-80843A0BAC95)
            dsm.Register(new DependencyProvider("network", "networkDrivers"));

            var count = 7;

            Assert.Equal(count, dsm.Participants.Count());

            Assert.Empty(dsm.Started);
            Assert.Equal(count, dsm.Stopped.Count());

            dsm.StartAsync().Wait();
            Assert.True(dsm.IsStarted);

            Assert.Equal(count, dsm.Started.Count());
            Assert.Empty(dsm.Stopped);

            dsm.StopAsync().Wait();
            Assert.False(dsm.IsStarted);

            Assert.Empty(dsm.Started);
            Assert.Equal(count, dsm.Stopped.Count());
        }

        [Fact]
        public void F_Cycle()
        {
            var dsm = new DependencyStateMachine();

            dsm.Register(new DependencyProvider("kernel"));
            dsm.Register(new DependencyProvider("hardwareLayer", "kernel"));
            dsm.Register(new DependencyContributor("networkDrivers", "eth0", "hardwareLayer"));
            dsm.Register(new DependencyContributor("networkDrivers", "eth1", "hardwareLayer"));
            dsm.Register(new DependencyContributor("networkDrivers", null, "hardwareLayer")); // Key: networkDrivers-(D4FB2F18-67B8-46C2-BF11-80843A0BAC95)
            dsm.Register(new DependencyProvider("network", "networkDrivers"));

            dsm.Register(new DependencyContributor("networkDrivers", "eth2", "hardwareLayer", "network")); // ERROR

            var count = 7;

            Assert.Equal(count, dsm.Participants.Count());

            Assert.Empty(dsm.Started);
            Assert.Equal(count, dsm.Stopped.Count());

            Exception ex = null;
            try
            {
                dsm.StartAsync().Wait();
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsType<AggregateException>(ex);
            var aex = (AggregateException)ex;
            Assert.Single(aex.InnerExceptions);
            Assert.IsType<DependenciesUnresolvableException>(aex.InnerExceptions.Single());
        }

    }
}
