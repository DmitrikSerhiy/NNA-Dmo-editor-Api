using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model.Entities;


namespace Persistence {
    public class NoNameContext : IdentityDbContext<NoNameUser, NoNameRole, Guid> {

        public DbSet<NoNameUser> ApplicationUsers { get; set; }
        // ReSharper disable UnusedMember.Global
        public DbSet<Dmo> Dmos { get; set; }

        public NoNameContext() { }

        public NoNameContext(DbContextOptions<NoNameContext> options)
            : base(options) {
        }
    }
}
