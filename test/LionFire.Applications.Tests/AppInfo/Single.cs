using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using LionFire.DependencyInjection;
using Xunit;

namespace AppInfo_
{
    // TODO: try multiple apps in one process?
    public class Single
    {
        [Fact]
        public async Task Pass()
        {
            await new AppHost()
                .AppInfo(new AppInfo
                {
                    CompanyName ="LionFireTest",
                    ProgramName = "LionFireTestProg",
                    AppDataDirName = "TestAppDataDir",
                })
                .Initialize()
                .RunNowAndWait(() =>
                {
                    var appInfo = InjectionContext.Current.GetService<AppInfo>();
                    
                    Assert.Equal("LionFireTest", appInfo.CompanyName);
                    Assert.Equal("LionFireTestProg", appInfo.ProgramName);
                    Assert.Equal("TestAppDataDir", appInfo.AppDataDirName);

                    Assert.Same(appInfo, AppInfo.CurrentAppInfo);
                    Assert.Same(appInfo, AppInfo.MainAppInfo);
                });
        }
    }
}
