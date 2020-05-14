using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTest
{
    class Program
    {
        private static IConfiguration configuration;

        static void Main(string[] args)
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            InsertData();
            PrintData();
        }

        private static void InsertData()
        {
            var options = new DbContextOptions<LibraryContext>();
            
            using (var context = new LibraryContext(configuration) {})
            {
                // Creates the database if not exists
                context.Database.EnsureCreated();

                // Adds a publisher
                var publisher = new Publisher
                {
                    Name = "Mariner Books"
                };
                context.Publisher.Add(publisher);
               
                // Adds some books
                var books = new List<Book> { 
                    new Book{
                        ISBN = "978-0544003415",
                        Title = "The Lord of the Rings",
                        Author = "J.R.R. Tolkien",
                        Language = "English",
                        Pages = 1216,
                        Publisher = publisher
                    },
                    new Book{
                        ISBN = "978-0547247762",
                        Title = "The Sealed Letter",
                        Author = "Emma Donoghue",
                        Language = "English",
                        Pages = 416,
                        Publisher = publisher
                    }
                };

                foreach(var book in books)
                    if (context.Book.Contains(book))
                        context.Book.UpdateRange(books);
                    else
                        context.Book.AddRange(books);

                // Saves changes
                context.SaveChanges();
            }
        }

        private static void PrintData()
        {
            // Gets and prints all books in database
            using (var context = new LibraryContext(configuration))
            {
                var books = context.Book
                  .Include(p => p.Publisher);
                foreach (var book in books)
                {
                    var data = new StringBuilder();
                    data.AppendLine($"ISBN: {book.ISBN}");
                    data.AppendLine($"Title: {book.Title}");
                    data.AppendLine($"Publisher: {book.Publisher.Name}");
                    Console.WriteLine(data.ToString());
                }
            }
        }
    }
}
