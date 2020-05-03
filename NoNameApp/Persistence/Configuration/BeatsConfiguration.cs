using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Persistence.Configuration {
    public static class BeatsConfiguration {
        public static void Configure(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Beat>()
                .HasOne(s => s.Dmo)
                .WithMany(g => g.Beats)
                .HasForeignKey(s => s.DmoId);
        }
    }
}
