using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Persistence {
    public class NoNameContext : IdentityDbContext<NoNameUser, NoNameRole, Guid> {

        public DbSet<NoNameUser> ApplicationUsers { get; set; }
        public DbSet<Dmo> Dmos { get; set; }
        public DbSet<UserDmoCollection> UserDmoCollections { get; set; }

        public NoNameContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Dmo>()
                .HasOne(d => d.UserDmoCollection)
                .WithMany(dc => dc.Dmos)
                .HasForeignKey(d => d.UserDmoCollectionId);

            modelBuilder.Entity<UserDmoCollection>()
                .HasOne(s => s.NoNameUser)
                .WithMany(g => g.UserDmoCollections)
                .HasForeignKey(s => s.NoNameUserId);

            modelBuilder.Entity<DmoUserDmoCollection>()
                .HasKey(dc => new { dc.DmoId, dc.UserDmoCollectionId });

            modelBuilder.Entity<DmoUserDmoCollection>()
                .HasOne(sc => sc.Dmo)
                .WithMany(s => s.DmoUserDmoCollections)
                .HasForeignKey(sc => sc.DmoId);

            modelBuilder.Entity<DmoUserDmoCollection>()
                .HasOne(sc => sc.UserDmoCollection)
                .WithMany(s => s.DmoUserDmoCollections)
                .HasForeignKey(sc => sc.UserDmoCollectionId);
        }
        public NoNameContext(DbContextOptions<NoNameContext> options)
            : base(options) {
        }
    }
}
