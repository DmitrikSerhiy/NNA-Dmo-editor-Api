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
        
        modelBuilder.Entity<Dmo>()
            .Property(b => b.ControllingIdeaId)
            .HasDefaultValue(0);
        
        modelBuilder.Entity<Dmo>()
            .Property(b => b.Published)
            .HasDefaultValue(false);

        modelBuilder.Entity<Dmo>()
            .HasMany(d => d.Conflicts)
            .WithOne(c => c.Dmo)
            .HasForeignKey(c => c.DmoId)
            .OnDelete(DeleteBehavior.NoAction)
            // ReSharper disable once RedundantArgumentDefaultValue
            .IsRequired(true);
    }
}