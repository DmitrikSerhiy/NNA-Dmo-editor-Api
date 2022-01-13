using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Persistence.Configuration {
    public static class UsersTokensConfiguration {
        
        public static void Configure(ModelBuilder modelBuilder) {
            modelBuilder
                .Entity<UsersTokens>()
                .ToView(nameof(UsersTokens))
                .HasNoKey();

            // not working for some reason. Migration file was edited manually.
            modelBuilder
                .Entity<NnaToken>(token => {
                    token.Property(p => p.TokenKeyId).IsRequired(true);
                });
        }
    }
}