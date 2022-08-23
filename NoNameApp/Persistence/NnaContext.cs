using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Persistence.Configuration;

namespace Persistence;
public class NnaContext : IdentityDbContext<NnaUser, NnaRole, Guid> {
    public DbSet<NnaUser> ApplicationUsers { get; set; }
    public DbSet<Dmo> Dmos { get; set; }
    public DbSet<DmoCollection> DmoCollections { get; set; }
        
    public DbSet<NnaLogin> Logins { get; set; }
    public DbSet<EditorConnection> EditorConnections { get; set; }
    public DbSet<NnaToken> Tokens { get; set; }
        
    public DbSet<Beat> Beats { get; set; }

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
    }
}