using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;

namespace NNA.Persistence.Configuration;

public static class CharacterConfiguration {
    public static void Configure(ModelBuilder modelBuilder) {
        modelBuilder.Entity<NnaMovieCharacter>()
            .HasOne(mch => mch.Dmo)
            .WithMany(mch => mch.Characters)
            .HasForeignKey(mch => mch.DmoId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        modelBuilder.Entity<NnaMovieCharacter>()
            .HasMany(mch => mch.Beats)
            .WithMany(mch => mch.Characters)
            .UsingEntity(j => j.ToTable("NnaCharactersBeats"));
    }
}
