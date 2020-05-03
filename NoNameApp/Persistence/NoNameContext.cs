using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Persistence.Configuration;

namespace Persistence {
    public class NoNameContext : IdentityDbContext<NoNameUser, NoNameRole, Guid> {

        public DbSet<NoNameUser> ApplicationUsers { get; set; }
        public DbSet<Dmo> Dmos { get; set; }
        public DbSet<Beat> Beats { get; set; }
        public DbSet<UserDmoCollection> UserDmoCollections { get; set; }

        public NoNameContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            DmoConfiguration.Configure(modelBuilder);
            UserDmoCollectionConfiguration.Configure(modelBuilder);
            DmoUserDmoCollectionConfiguration.Configure(modelBuilder);
            BeatsConfiguration.Configure(modelBuilder);
        }
        public NoNameContext(DbContextOptions<NoNameContext> options)
            : base(options) {
        }
    }
}
