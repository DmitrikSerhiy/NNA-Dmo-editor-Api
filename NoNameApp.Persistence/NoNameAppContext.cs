using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoNameApp.Infrastructure;

namespace NoNameApp.Persistence {
    public class NoNameAppContext : IdentityDbContext<AppUser, AppRole, Guid> {

        public NoNameAppContext() { }
        public NoNameAppContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

        }
    }
}
