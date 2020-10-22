//using LionFire.DependencyMachines;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using System;
//#if UNITY
//using UnityEngine;
//#endif
//using System.Threading;
//using System.Threading.Tasks;

//namespace LionFire.Shell
//{
//    public class LionFireApp : IHostedService
//    {
//        #region Dependencies

//        public IServiceProvider ServiceProvider { get; }

//        #endregion

//        #region Construction

//        public LionFireApp(IServiceProvider serviceProvider)
//        {
//            ServiceProvider = serviceProvider;
//            ActivatorUtilities.CreateInstance<DependencyStateMachine>(serviceProvider,
//                            name == null ? Array.Empty<object>() : new object[] { name });

//        }

//        #endregion

//        public Task StartAsync(CancellationToken cancellationToken)
//        {

//        }

//        public Task StopAsync(CancellationToken cancellationToken)
//        {

//        }

//    }
//}
