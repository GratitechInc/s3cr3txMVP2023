using System;
using Microsoft.EntityFrameworkCore;
using s3cr3tx.Models;

namespace s3cr3tx.Models
{
    public class LoginDbContext : DbContext
    {
        public LoginDbContext(DbContextOptions<LoginDbContext> options)
            : base(options)
        {
        }

        //private readonly string _connectionString;

        //public S3cr3txDbContext(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}

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

        public DbSet<Login> Logins { get; set; }
    }
}
