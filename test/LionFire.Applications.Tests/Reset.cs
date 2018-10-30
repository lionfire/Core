using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using LionFire.Serialization;
using Xunit;

namespace LionFire.Applications.Tests
{
    public class Reset_ManualSingleton
    {
        [Fact]
        public async void Fail()
        {
            AppHost.DetectUnitTestMode = false;

            AppHost.Reset();
            await No_SerializationProvider();

            AppHost.Reset();
            await With_SerializationProvider();

            // MISSING: AppHost.Reset();
            await No_SerializationProvider(expectFail: true);
        }

        [Fact]
        public async void Pass()
        {
            AppHost.DetectUnitTestMode = false;

            AppHost.Reset();
            await No_SerializationProvider();

            AppHost.Reset();
            await With_SerializationProvider();

            AppHost.Reset();
            await No_SerializationProvider();
        }

        [Fact]
        public async void Pass_UnitTestMode()
        {
            AppHost.Reset();
            AppHost.DetectUnitTestMode = true;

            await No_SerializationProvider();
            await With_SerializationProvider();
            await No_SerializationProvider();
        }

        #region Utilities

        private async Task No_SerializationProvider(bool expectFail = false, [CallerMemberName] string callerName = null)
        {
            var app = new AppHost(callerName + $" No_SerializationProvider({expectFail})");
            
            await app.RunNowAndWait(() =>
            {
                
                if(!expectFail && !AppHost.DetectUnitTestMode)
                {
                    Assert.Equal(app.Name, AppHost.MainApp.Name);
                }
                else
                {
                    Assert.NotEqual(app.Name, AppHost.MainApp.Name);
                }
                
                var ser = Defaults.TryGet<ISerializationProvider>();
                if (!expectFail)
                {
                    Assert.Null(ser);
                }
                else
                {
                    Assert.NotNull(ser);
                }
            });
        }

        private async Task With_SerializationProvider([CallerMemberName] string callerName = null)
        {
            var app = new AppHost(callerName + $" With_SerializationProvider");

            await app
                .AddSerialization()
                .RunNowAndWait(() =>
                {
                    if(!AppHost.DetectUnitTestMode) Assert.Equal(app.Name, AppHost.MainApp.Name);
                    var ser = Defaults.TryGet<ISerializationProvider>();
                    Assert.NotNull(ser);
                })
            ;
        }

        #endregion

    }
}
