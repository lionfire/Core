using LionFire.DependencyMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Gates_
{

    public class NamedGate_
    {
        [Fact]
        public void P_ManualResetEvent()
        {
            var dsm = new DependencyStateMachine();


            ManualDependencyContributor alpha1, alpha2;
            dsm.Register(alpha1 = new ManualDependencyContributor("alpha"));
            dsm.Register(alpha2 = new ManualDependencyContributor("alpha"));

            ManualDependencyContributor bravo1, bravo2;
            dsm.Register(bravo1 = new ManualDependencyContributor("bravo"));
            dsm.Register(bravo2 = new ManualDependencyContributor("bravo"));

            var count = 4;


            // TODO: actually test the reset events
            // TODO: make bravo depend on alpha
            //dsm.AddDependency("bravo", "alpha");

            alpha1.StartResetEvent.Set();
            alpha2.StartResetEvent.Set();
            bravo1.StartResetEvent.Set();
            bravo2.StartResetEvent.Set();

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
    }
}
