using ConsoleDump;
using Rhetos.Configuration.Autofac;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.Logging;
using Rhetos.Utilities;
using System;
using System.IO;
using System.Linq;

namespace Bookstore.Playground
{
    class Program
    {
        static void Main(string[] args)
        {

            ConsoleLogger.MinLevel = EventType.Info; // Use "Trace" for more detailed log.
            var rhetosServerPath = @"..\..\..\..\..\dist\BookStoreRhetosServer\RhetosServer.2.12.0";
            Directory.SetCurrentDirectory(rhetosServerPath);
            // Set commitChanges parameter to COMMIT or ROLLBACK the data changes.
            using (var container = new RhetosTestContainer(commitChanges: false))
            {
                var context = container.Resolve<Common.ExecutionContext>();
                var repository = context.Repository;

                // Query data from the `Common.Claim` table:

                var claims = repository.Common.Claim.Query()
                    .Where(c => c.ClaimResource.StartsWith("Common.") && c.ClaimRight == "New")
                    .ToSimple(); // Removes ORM navigation properties from the loaded objects.

                claims.ToString().Dump("Common.Claims SQL query");
                claims.Dump("Common.Claims items");

                // Add and remove a `Common.Principal`:

                var testUser = new Common.Principal { Name = "Test123", ID = Guid.NewGuid() };
                repository.Common.Principal.Insert(new[] { testUser });
                repository.Common.Principal.Delete(new[] { testUser });

                // Print logged events for the `Common.Principal`:

                repository.Common.LogReader.Query()
                    .Where(log => log.TableName == "Common.Principal" && log.ItemId == testUser.ID)
                    .ToList()
                    .Dump("Common.Principal log");

                Console.WriteLine("Done.");
                Console.ReadLine();
            }

        }
    }
}
