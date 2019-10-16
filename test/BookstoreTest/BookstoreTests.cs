using Bookstore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhetos.Configuration.Autofac;
using Rhetos.Dom.DefaultConcepts;
using System;
using System.IO;
using System.Linq;

namespace BookstoreTest
{
    [TestClass]
    public class BookstoreTests
    {
        [TestMethod]
        public void BookInfoNoComments()
        {
            var rhetosServer = @"C:\Users\mtomaic\source\repos\BookStore\dist\BookStoreRhetosServer\RhetosServer.2.12.0";
            Directory.SetCurrentDirectory(rhetosServer);

            using (var rhetos = new RhetosTestContainer())
            {
                var repository = rhetos.Resolve<Common.DomRepository>();

                var book = new Book { Title = "testna knjiga" };

                repository.Bookstore.Book.Insert(book);               
                Assert.AreEqual(0, numberOfComments(repository, book.ID));

                var comment1 = new Comment { BookID = book.ID, Text = "comm" };
                repository.Bookstore.Comment.Insert(comment1);
                Assert.AreEqual(1, numberOfComments(repository, book.ID));
            }
        }

        private int? numberOfComments(Common.DomRepository repository, Guid bookId)
        {
            return repository.Bookstore.BookInfo
                   .Query(bi => bi.ID == bookId)
                   .Select(bi => bi.NumberOfComments).Single();
        }
    }
}
