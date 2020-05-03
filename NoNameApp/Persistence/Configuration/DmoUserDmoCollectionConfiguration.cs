using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Persistence.Configuration {
    public static class DmoUserDmoCollectionConfiguration {
        public static void Configure(ModelBuilder modelBuilder) {
            modelBuilder.Entity<DmoUserDmoCollection>()
                .HasKey(dc => new { dc.DmoId, dc.UserDmoCollectionId });

            modelBuilder.Entity<DmoUserDmoCollection>()
                .HasOne(sc => sc.Dmo)
                .WithMany(s => s.DmoUserDmoCollections)
                .HasForeignKey(sc => sc.DmoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DmoUserDmoCollection>()
                .HasOne(sc => sc.UserDmoCollection)
                .WithMany(s => s.DmoUserDmoCollections)
                .HasForeignKey(sc => sc.UserDmoCollectionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
