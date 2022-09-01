using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;

namespace NNA.Persistence.Configuration;

public static class UsersTokensConfiguration {
    public static void Configure(ModelBuilder modelBuilder) {
        modelBuilder
            .Entity<UsersTokens>()
            .ToView(nameof(UsersTokens))
            .HasNoKey();

        // todo: not working for some reason. Migration file was edited manually.
        modelBuilder
            .Entity<NnaToken>(token => {
                // ReSharper disable once RedundantArgumentDefaultValue
                token.Property(p => p.TokenKeyId).IsRequired(true);
            });
    }
}