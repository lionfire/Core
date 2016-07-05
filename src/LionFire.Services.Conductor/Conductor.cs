using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Services
{
    public class Conductor : IService
    {
        public Conductor()
        {
        }

        public Task Start()
        {
            // Load default configuration: services to initialize
            // Initialize all services

            return Task.CompletedTask;
        }

        public Task Stop(StopMode stopMode)
        {


            return Task.CompletedTask;
        }
    }
}
