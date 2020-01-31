using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Persistence {
    public class NoNameContext : IdentityDbContext<NoNameUser, NoNameRole, Guid> {

        public DbSet<NoNameUser> ApplicationUsers { get; set; }
        // ReSharper disable UnusedMember.Global
        public DbSet<Dmo> Dmos { get; set; }
        public DbSet<UserDmoCollection> UserDmoCollections { get; set; }
        public DbSet<DmoUserDmoCollection> DmoUserDmoCollections { get; set; }

        public NoNameContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserDmoCollection>()
                .HasOne(s => s.NoNameUser)
                .WithMany(g => g.UserDmoCollections)
                .HasForeignKey(s => s.NoNameUserId);


            modelBuilder.Entity<DmoUserDmoCollection>()
                .HasKey(dc => new { dc.DmoId, dc.UserDmoCollectionId });
            //todo: migrate all the configuration to this method with extension methods and fluent API
        }
        public NoNameContext(DbContextOptions<NoNameContext> options)
            : base(options) {
        }
    }
}
