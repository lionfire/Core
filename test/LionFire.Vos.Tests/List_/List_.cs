#if false // TODO After Write_ tests
using LionFire.Hosting;
using System;
using LionFire.Services;
using System.Collections.Generic;
using System.Text;
using Xunit;
using LionFire.Services;
using LionFire.Vos;
using LionFire.Vos.Environment;
using LionFire.Resolves.ChainResolving;
using LionFire.FlexObjects;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using LionFire.Persistence.Testing;
using LionFire.Referencing;

namespace List_
{
   
    public class List_
    {
        private readonly ITestOutputHelper output;
        
        public List_(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async void P_List_Heterogeneous()
        {
            await VosTestHost.Create()
                .RunAsync(async serviceProvider =>
                {
                    var root = serviceProvider.GetRootVob();
                    var test = "$test".ToVosReference();

                    #region Setup

                    var child1 = test.GetChild("child1").GetReadWriteHandle<TestClass1>();
                    var child2 = test.GetChild("child2").GetReadWriteHandle<TestClass2>();

                    child1.Value = new TestClass1();
                    await child1.Put();
                    child2.Value = new TestClass2();
                    await child2.Put();

                    #endregion

                    var hList = test.GetListHandle();

                    var result = await hList.Resolve();

                    Assert.NotNull(result.Value);
                    Assert.NotEmpty(result.Value);
                    //Assert.Equal("value1", root.Environment()["key1"]);
                    
                });
        }

        [Fact]
        public async void P_GetList_GetEnumerableHandle_Same()
        {
            await VosTestHost.Create()
                .RunAsync(async serviceProvider =>
                {
                    var root = serviceProvider.GetRootVob();
                    var test = "$test".ToVosReference();
                    
                    var hList1 = test.GetReadHandle<IEnumerable<Listing>>();
                    var hList2 = test.GetListHandle();
                    Assert.Equal(hList1.Key, hList2.Key);
                    Assert.Equal(hList1.Reference.Key, hList2.Reference.Key);
                    Assert.Equal(hList1.Reference.Path, hList2.Reference.Path);
                });
        }
    }
}
#endif