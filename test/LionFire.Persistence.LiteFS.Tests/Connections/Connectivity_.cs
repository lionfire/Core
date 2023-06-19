using LionFire.Data.LiteDB.Connections;
using LionFire.Dependencies;
using LionFire.Hosting;
using LiteDB;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using LionFire;
using LionFire.Data;
using LionFire.Persistence.LiteFs.Tests;

namespace ConnectionManager_
{
    public class Connectivity_
    {
        static public IEnumerable<object[]> ConnectionParameters => ConnectionOptionsInputData.AllForXUnit;

        [Theory]
        [MemberData(nameof(ConnectionParameters))]
        public async void CreateWriteReadDelete(LionFire.Data.LiteDB.Connections.LiteDbConnectionOptions connectionOptions)
        {
            var testGuid = Guid.NewGuid();

            await PersistersHost.Create()
                .AddLiteFs()
            //.ConfigureServices((_, services) =>
            //{
            //    services
            //})
            //.ConfigureHostConfiguration(config =>
            //{
            //    config.AddInMemoryCollection(new Dictionary<string, string>()
            //    {
            //        { "LiteDb:ConnectionStrings:" + testGuid, connectionOptions.ConnectionString }
            //    });
            //})
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "LiteDb:ConnectionStrings:" + testGuid, connectionOptions.ConnectionString }
                });
            })
            .RunAsync(async () =>
            {
                if (connectionOptions.Path != null)
                {
                    Assert.False(File.Exists(connectionOptions.Path));
                }

                var cleanup = true;

                var connectionManager = DependencyContext.Current.GetService<LiteDbConnectionManager>();

                var db = (await connectionManager.GetConnection(testGuid.ToString())).DB;

                var col = db.GetCollection<Customer>(typeof(Customer).Name);

                var customer = SampleData.GetCustomer();

                col.Insert(customer);

                customer.Name = "Jane Doe";

                var phone = customer.Phones[0];

                col.Update(customer);

                col.EnsureIndex(x => x.Name);

                var results = col.Query()
                    .Where(x => x.Name.StartsWith("J"))
                    .OrderBy(x => x.Name)
                    .Select(x => new { x.Name, NameUpper = x.Name.ToUpper() })
                    .Limit(10)
                    .ToList();

                // Let's create an index in phone numbers (using expression). It's a multikey index
                col.EnsureIndex(x => x.Phones);

                // and now we can query phones
                var r = col.FindOne(x => x.Phones.Contains(phone));

                Assert.NotNull(r);


                //FileReference reference = dbPath;
                //Assert.Equal(reference.Path, dbPath.Replace('\\', '/'));

                //#region Write Test Contents

                //var testContents = "testing123";
                //File.WriteAllText(dbPath, testContents);
                //Assert.True(File.Exists(dbPath));

                //#endregion

                //#region Retrieve (Primary assertion)

                //var readHandle = reference.GetReadHandle<string>();
                ////var persistenceResult = await readHandle.Retrieve();
                //var persistenceResult = await readHandle.Resolve() as ITransferResult;

                //Assert.True(persistenceResult.Flags.HasFlag(TransferResultFlags.Success));
                //Assert.Equal(testContents, readHandle.Value);

                //#endregion

                #region Cleanup

                if (cleanup)
                {
                    if (connectionOptions.Path != null && File.Exists(connectionOptions.Path))
                    {
                        File.Delete(connectionOptions.Path);
                    }
                    Assert.False(File.Exists(connectionOptions.Path));
                }

                #endregion
            });
        }
    }
}
