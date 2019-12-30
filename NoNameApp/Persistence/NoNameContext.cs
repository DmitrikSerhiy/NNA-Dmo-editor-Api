using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model;

namespace Persistence {
    public class NoNameContext : IdentityDbContext<NoNameUser, NoNameRole, Guid> {

        public DbSet<NoNameUser> ApplicationUsers { get; set; }

        public DbSet<DecompositionNode> DecompositionNode { get; set; }

        public NoNameContext() { }

        public NoNameContext(DbContextOptions<NoNameContext> options)
            : base(options) {
        }
    }
}
