using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;

namespace NNA.Persistence.Configuration;
public static class EditorConnectionConfiguration {
    public static void Configure(ModelBuilder modelBuilder) {
        modelBuilder.Entity<EditorConnection>()
            .HasKey(s => new { s.ConnectionId, s.UserId });

        modelBuilder.Entity<EditorConnection>()
            .HasIndex(s => s.UserId);

    }
}