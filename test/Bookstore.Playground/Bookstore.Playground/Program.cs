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
            using (var container = new RhetosTestContainer(commitChanges: true))
            {
                var context = container.Resolve<Common.ExecutionContext>();
                var repository = context.Repository;

                //var allBooks = repository.Bookstore.Book.Load(t => t.NumberOfPages > 200).Dump();
                //var someBooks = repository.Bookstore.Book.Query().ToSimple().Dump();

                //var book = repository.Bookstore.Book.Query(new[] { new Guid("8FCD6DAA-27F1-4ABB-9556-5E0C74BA00F0") }).ToSimple();

                var filter = new FilterCriteria("Title", "StartsWith", "osi");
                var filteredBook = repository.Bookstore.Book.Query(filter);

                var booksWithComments = repository.Bookstore.Comment.Query().Where(c => c.BookID != null).ToString();

                repository.Bookstore.Book.Insert(new Bookstore.Book { Title = "new book", NumberOfPages = 201, Code = "A+" });

                // homework

                //Load
                var books = repository.Bookstore.Book.Load();
                var authors = repository.Bookstore.Person.Load();

                // left join
                //var result = from book in books
                //             join author in authors on book.AuthorID equals author.ID into bookAuthor
                //             from subAuthor in bookAuthor.DefaultIfEmpty()
                //             select new { book.Title, AuthorName = subAuthor?.Name ?? "" }.Dump();
                // inner join
                var booksWithAuthors = books.Join(repository.Bookstore.Person.Load(), currentBook => currentBook.AuthorID, author => author.ID, (currentBook, author) => new { currentBook.Title, author.Name }).Dump();

                // with load
                var booksAndAuthors = books.Select(b => new
                {
                    b.Title,
                    repository.Bookstore.Person.Load(new Guid[] { b.AuthorID ?? Guid.Empty }).SingleOrDefault()?.Name
                }).Dump();

                //Query
                var booksWithAuthorsQuery = repository.Bookstore.Book.Query().Select(b => new { b.Title, b.Author.Name }).Dump();

                //ToString
                var booksWithAuthorsSqlQuery = repository.Bookstore.Book.Query().Select(b => new { b.Title, b.Author.Name }).ToString().Dump();


                // homework 3
                var filterParameter = new Bookstore.BooksWithMoreThan100Pages();
                var query = repository.Bookstore.Book.Query(filterParameter);
                query.ToString().Dump();
                query.ToSimple().ToList().Dump();

                var composableFilterParameter = new Bookstore.BooksWithMoreThan50Pages();
                var composableQuery = repository.Bookstore.Book.Query(composableFilterParameter);
                composableQuery.ToString().Dump();
                composableQuery.ToSimple().ToList().Dump();

                var complex = new Bookstore.ComplexSearch();
                complex.NumberOfPages = 20;
                complex.IsForeignBook = true;
                var complexQuery = repository.Bookstore.Book.Load(complex);
                complexQuery.Dump();





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
