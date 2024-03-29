﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;
using NNA.Persistence.Configuration;

namespace NNA.Persistence;

public sealed class NnaContext : IdentityDbContext<NnaUser, IdentityRole<Guid>, Guid> {
    public DbSet<NnaUser> ApplicationUsers => Set<NnaUser>();
    public DbSet<Dmo> Dmos => Set<Dmo>();
    public DbSet<DmoCollection> DmoCollections => Set<DmoCollection>();
    public DbSet<NnaLogin> Logins => Set<NnaLogin>();
    public DbSet<EditorConnection> EditorConnections => Set<EditorConnection>();
    public DbSet<NnaToken> Tokens => Set<NnaToken>();
    public DbSet<Beat> Beats => Set<Beat>();
    public DbSet<NnaMovieCharacter> Characters => Set<NnaMovieCharacter>();
    public DbSet<NnaMovieCharacterInBeat> CharacterInBeats => Set<NnaMovieCharacterInBeat>();
    public DbSet<NnaMovieCharacterConflictInDmo> NnaMovieCharacterConflicts => Set<NnaMovieCharacterConflictInDmo>();
    public DbSet<NnaTag> Tags => Set<NnaTag>();
    public DbSet<NnaTagInBeat> TagInBeats => Set<NnaTagInBeat>();
    
    public NnaContext(DbContextOptions<NnaContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        DmoConfiguration.Configure(modelBuilder);
        UserDmoCollectionConfiguration.Configure(modelBuilder);
        DmoUserDmoCollectionConfiguration.Configure(modelBuilder);
        UsersTokensConfiguration.Configure(modelBuilder);
        EditorConnectionConfiguration.Configure(modelBuilder);
        BeatConfiguration.Configure(modelBuilder);
        CharacterConfiguration.Configure(modelBuilder);
    }
}