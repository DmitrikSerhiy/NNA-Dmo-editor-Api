using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;

namespace NNA.Persistence.Configuration;

public static class DmoConfiguration {
    public static void Configure(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Dmo>()
            .HasOne(d => d.NnaUser)
            .WithMany(dc => dc.Dmos)
            .HasForeignKey(d => d.NnaUserId);

        modelBuilder.Entity<Dmo>()
            .HasMany(d => d.Beats)
            .WithOne(d => d.Dmo)
            .HasForeignKey(d => d.DmoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}