using Microsoft.EntityFrameworkCore;
using Monovi.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monovi.DataAccess.Context
{
   public  class MonoviDbContext : DbContext
    {
        public MonoviDbContext(DbContextOptions<MonoviDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();
            base.OnModelCreating(modelBuilder);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
