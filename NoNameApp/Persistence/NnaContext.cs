using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Persistence.Configuration;

namespace Persistence {
    public class NnaContext : IdentityDbContext<NnaUser, NnaRole, Guid> {

        public DbSet<NnaUser> ApplicationUsers { get; set; }
        public DbSet<Dmo> Dmos { get; set; }
        public DbSet<DmoCollection> DmoCollections { get; set; }
        
        public DbSet<NnaLogin> Logins { get; set; }

        public NnaContext() { }

        public NnaContext(DbContextOptions<NnaContext> options)
            : base(options) { }


        // todo: remove it later
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=nna-local");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            DmoConfiguration.Configure(modelBuilder);
            UserDmoCollectionConfiguration.Configure(modelBuilder);
            DmoUserDmoCollectionConfiguration.Configure(modelBuilder);
        }
    }
}
