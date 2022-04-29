using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Persistence.Configuration {
    public static class BeatConfiguration {
        public static void Configure(ModelBuilder modelBuilder) {

            modelBuilder.Entity<Beat>()
                .HasOne(d => d.User)
                .WithMany(dc => dc.Beats)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Beat>()
                .HasOne(d => d.Dmo)
                .WithMany(dc => dc.Beats)
                .HasForeignKey(d => d.DmoId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}