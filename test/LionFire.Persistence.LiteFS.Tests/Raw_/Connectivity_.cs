using LionFire.Hosting;
using LionFire.Persistence.LiteFs.Tests;
using System;
using System.IO;
using Xunit;
using LiteDB;
using System.Collections.Generic;
using LionFire.LiteDb;
using System.Linq;
using System.Text;

class TestOptions
{
    public static bool CleanupAfter { get; set; } = true;
}
namespace Raw_
{

    public partial class Connectivity_
    {
        static public IEnumerable<object[]> ConnectionParameters => ConnectionOptionsInputData.AllForXUnit;


        [Theory]
        [MemberData(nameof(ConnectionParameters))]
        public async void CreateWriteReadDelete(LionFire.Data.LiteDB.Connections.LiteDbConnectionOptions connectionOptions)
        {

            await PersistersHost.Create()
                .AddLiteFs()
            //.ConfigureServices((_, services) =>
            //{
            //    services
            //})
            .RunAsync(() =>
           {
               if (connectionOptions.Path != null)
               {
                   Assert.False(File.Exists(connectionOptions.Path));
               }

               #region LiteDB

               using (var db = new LiteDatabase(connectionOptions.ConnectionString))
               {
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
               }

               #endregion

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
               //var persistenceResult = await readHandle.Resolve() as IPersistenceResult;

               //Assert.True(persistenceResult.Flags.HasFlag(PersistenceResultFlags.Success));
               //Assert.Equal(testContents, readHandle.Value);

               //#endregion

               #region Cleanup

               if (TestOptions.CleanupAfter)
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
