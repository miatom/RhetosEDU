﻿using ConsoleDump;
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
            using (var container = new RhetosTestContainer(commitChanges: true))
            {
                var context = container.Resolve<Common.ExecutionContext>();
                var repository = context.Repository;

                var allBooks = repository.Bookstore.Book.Load(t => t.NumberOfPages > 200).Dump();
                var someBooks = repository.Bookstore.Book.Query().ToSimple().Dump();

                var book = repository.Bookstore.Book.Query(new[] { new Guid("8FCD6DAA-27F1-4ABB-9556-5E0C74BA00F0") }).ToSimple();

                var filter = new FilterCriteria("Title", "StartsWith", "osi");
                var filteredBook = repository.Bookstore.Book.Query(filter);

                var booksWithComments = repository.Bookstore.Comment.Query().Where(c => c.BookID != null).ToString();

                repository.Bookstore.Book.Insert(new Bookstore.Book { Title = "new book", NumberOfPages = 201, Code = "A+" });

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