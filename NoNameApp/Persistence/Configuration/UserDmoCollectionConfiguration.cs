using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Persistence.Configuration {
    public static class UserDmoCollectionConfiguration {
        public static void Configure(ModelBuilder modelBuilder) {
            modelBuilder.Entity<UserDmoCollection>()
                .HasOne(s => s.NoNameUser)
                .WithMany(g => g.UserDmoCollections)
                .HasForeignKey(s => s.NoNameUserId);
        }
    }
}
