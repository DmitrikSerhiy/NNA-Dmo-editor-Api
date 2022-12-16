using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;

namespace NNA.Persistence.Configuration;

public static class TagConfiguration
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NnaTagInBeat>()
            .HasOne(tib => tib.Beat)
            .WithMany(b => b.Tags)
            .HasForeignKey(tib => tib.BeatId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<NnaTagInBeat>()
            .HasOne(tib => tib.Tag)
            .WithMany(b => b.Beats)
            .HasForeignKey(tib => tib.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NnaTagInBeat>()
            .HasKey(mchInBeat => mchInBeat.Id);
    }
}