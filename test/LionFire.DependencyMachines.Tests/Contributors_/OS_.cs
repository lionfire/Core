using LionFire.DependencyMachines;
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


            dsm.Register(new Provider("kernel"));
            dsm.Register(new Provider("hardwareLayer", "kernel"));
            dsm.Register(new Contributor("networkDrivers", "eth0", "hardwareLayer"));
            dsm.Register(new Contributor("networkDrivers", "eth1", "hardwareLayer"));
            dsm.Register(new Contributor("networkDrivers", "eth2", "hardwareLayer"));
            dsm.Register(new Contributor("networkDrivers", null, "hardwareLayer")); // Key: networkDrivers-(D4FB2F18-67B8-46C2-BF11-80843A0BAC95)
            dsm.Register(new Provider("network", "networkDrivers"));

            var count = 7;

            Assert.Equal(count, dsm.Participants.Count());

            Assert.Empty(dsm.StartedParticipants);
            Assert.Equal(count, dsm.StoppedParticipants.Count());

            dsm.StartAsync().Wait();
            Assert.True(dsm.IsStarted);

            Assert.Equal(count, dsm.StartedParticipants.Count());
            Assert.Empty(dsm.StoppedParticipants);

            dsm.StopAsync().Wait();
            Assert.False(dsm.IsStarted);

            Assert.Empty(dsm.StartedParticipants);
            Assert.Equal(count, dsm.StoppedParticipants.Count());
        }

        [Fact]
        public void F_Cycle()
        {
            var dsm = new DependencyStateMachine();

            dsm.Register(new Provider("kernel"));
            dsm.Register(new Provider("hardwareLayer", "kernel"));
            dsm.Register(new Contributor("networkDrivers", "eth0", "hardwareLayer"));
            dsm.Register(new Contributor("networkDrivers", "eth1", "hardwareLayer"));
            dsm.Register(new Contributor("networkDrivers", null, "hardwareLayer")); // Key: networkDrivers-(D4FB2F18-67B8-46C2-BF11-80843A0BAC95)
            dsm.Register(new Provider("network", "networkDrivers"));

            dsm.Register(new Contributor("networkDrivers", "eth2", "hardwareLayer", "network")); // ERROR

            var count = 7;

            Assert.Equal(count, dsm.Participants.Count());

            Assert.Empty(dsm.StartedParticipants);
            Assert.Equal(count, dsm.StoppedParticipants.Count());

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
