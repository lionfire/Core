using LionFire.Hosting;
using System;
using System.Linq;
using LionFire.Services;
using System.Collections.Generic;
using System.Text;
using Xunit;
using LionFire.Vos;
using LionFire.Vos.Environment;
using LionFire.Resolves.ChainResolving;
using LionFire.FlexObjects;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using LionFire.Persistence.Testing;
using LionFire.Referencing;
using System.Threading;
using LionFire.Persistence;

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

                    var child1 = test.GetChild("child1.json").GetReadWriteHandle<TestClass1>();
                    var child2 = test.GetChild("child2.json").GetReadWriteHandle<TestClass2>();

                    child1.Value = TestClass1.Create;
                    await child1.Put();
                    child2.Value = new TestClass2() { IntProp2 = 12, StringProp2 = "abc" };
                    await child2.Put();

                    #endregion

                    var hList = test.GetListHandle();

                    var result = await hList.Resolve();

                    var listings = result.Value.Value;
                    Assert.NotNull(listings);
                    Assert.NotEmpty(listings);

                    Assert.Equal("child1.json", listings.ElementAt(0).Name);
                    Assert.False(listings.ElementAt(0).IsDirectory);

                    Assert.Equal("child2.json", listings.ElementAt(1).Name);
                    Assert.False(listings.ElementAt(1).IsDirectory);

                });
        }

        [Fact]
        public async void P_GetList_GetEnumerableHandle_Same()
        {
            await VosTestHost.Create()
                .RunAsync(serviceProvider =>
                {
                    var root = serviceProvider.GetRootVob();
                    var test = "$test".ToVosReference();
                    
                    var hList1 = test.GetReadHandle<Metadata<IEnumerable<Listing>>>();
                    var hList2 = test.GetListHandle();
                    Assert.Equal(hList1.Key, hList2.Key);
                    Assert.Equal(hList1.Reference.Key, hList2.Reference.Key);
                    Assert.Equal(hList1.Reference.Path, hList2.Reference.Path);
                });
        }
    }
}
