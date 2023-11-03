using System;
using Microsoft.EntityFrameworkCore;
using s3cr3tx.Models;

namespace s3cr3tx.Models
{
    public class S3cr3txDbContext : DbContext
    {
        public S3cr3txDbContext(DbContextOptions<S3cr3txDbContext> options)
            : base(options)
        {
        }

        private readonly string _connectionString;

        public S3cr3txDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (_connectionString == "")
        //    {
        //        optionsBuilder.UseSqlServer(@"Server=.;Database=s3cr3tx;Integrated Security=SSPI");
        //    }
        //    else {
        //        optionsBuilder.UseSqlServer(_connectionString);
        //    }
        //}

        public DbSet<S3cr3tx> s3cr3tx { get; set; }
    }
}
