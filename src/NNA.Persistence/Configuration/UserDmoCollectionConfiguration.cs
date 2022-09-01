using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;

namespace NNA.Persistence.Configuration;

public static class UserDmoCollectionConfiguration {
    public static void Configure(ModelBuilder modelBuilder) {
        modelBuilder.Entity<DmoCollection>()
            .HasOne(s => s.NnaUser)
            .WithMany(g => g.DmoCollections)
            .HasForeignKey(s => s.NnaUserId);
    }
}