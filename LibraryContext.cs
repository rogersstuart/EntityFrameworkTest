using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Options;

namespace EFTest
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Book { get; set; }

        public DbSet<Publisher> Publisher { get; set; }

        private IConfiguration configuration = null;
        private string provider;

        public LibraryContext(IConfiguration configuration) { this.configuration = configuration; }
        public LibraryContext(string provider) { this.provider = provider; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           switch(configuration != null ? configuration["Provider"] : provider)
            {
                case "MySQL":   optionsBuilder.UseMySql("server=localhost;database=library;user=your;password=login");
                                break;
                case "SQLite":  optionsBuilder.UseSqlite("sqlite.db");
                                break;
                default:        throw new Exception("Invalid Provider Specified");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Publisher>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.ISBN);
                entity.Property(e => e.Title).IsRequired();
                entity.HasOne(d => d.Publisher)
                  .WithMany(p => p.Books);
            });
        }
    }
}
