using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;

namespace NNA.Persistence.Configuration;
public static class DmoUserDmoCollectionConfiguration {
    public static void Configure(ModelBuilder modelBuilder) {

        modelBuilder.Entity<DmoCollectionDmo>()
            .HasKey(dc => new { dc.DmoId, dc.DmoCollectionId });

        modelBuilder.Entity<DmoCollectionDmo>()
            .HasOne(dc => dc.Dmo)
            .WithMany(dc => dc.DmoCollectionDmos)
            .HasForeignKey(sc => sc.DmoId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DmoCollectionDmo>()
            .HasOne(dc => dc.DmoCollection)
            .WithMany(dc => dc.DmoCollectionDmos)
            .HasForeignKey(sc => sc.DmoCollectionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}