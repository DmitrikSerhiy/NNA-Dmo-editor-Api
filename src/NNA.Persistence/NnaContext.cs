using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;
using NNA.Persistence.Configuration;

namespace NNA.Persistence;

public class NnaContext : IdentityDbContext<NnaUser, NnaRole, Guid> {
    public DbSet<NnaUser> ApplicationUsers => Set<NnaUser>();
    public DbSet<Dmo> Dmos => Set<Dmo>();
    public DbSet<DmoCollection> DmoCollections => Set<DmoCollection>();
    public DbSet<NnaLogin> Logins => Set<NnaLogin>();
    public DbSet<EditorConnection> EditorConnections => Set<EditorConnection>();
    public DbSet<NnaToken> Tokens => Set<NnaToken>();
    public DbSet<Beat> Beats => Set<Beat>();

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