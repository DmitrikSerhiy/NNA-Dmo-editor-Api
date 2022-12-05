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
            // ReSharper disable once RedundantArgumentDefaultValue
            .IsRequired(true);

        modelBuilder.Entity<NnaMovieCharacter>()
            .Property(b => b.Color)
            .HasMaxLength(7)
            .HasDefaultValue("#000000");

        modelBuilder.Entity<NnaMovieCharacterInBeat>()
            .HasOne(mchInBeat => mchInBeat.Beat)
            .WithMany(b => b.Characters)
            .HasForeignKey(mchInBeat => mchInBeat.BeatId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<NnaMovieCharacterInBeat>()
            .HasOne(mchInBeat => mchInBeat.Character)
            .WithMany(cha => cha.Beats)
            .HasForeignKey(mchInBeat => mchInBeat.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NnaMovieCharacterInBeat>()
            .HasKey(mchInBeat => mchInBeat.Id);
        
        modelBuilder.Entity<NnaMovieCharacterConflictInDmo>()
            .HasOne(c => c.Character)
            .WithMany(cha => cha.Conflicts)
            .HasForeignKey(c => c.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<NnaMovieCharacterConflictInDmo>()
            .Property(b => b.Achieved)
            .HasDefaultValue(0);
        
        modelBuilder.Entity<NnaMovieCharacterConflictInDmo>()
            .Property(b => b.CharacterType)
            .HasDefaultValue(1);
        
        
    }
}
