using Microsoft.EntityFrameworkCore;
using Model;

namespace Persistence {
    public class NoNameContext : DbContext {

        public DbSet<DecompositionNode> DecompositionNode { get; set; }

        public NoNameContext(DbContextOptions<NoNameContext> options)
            : base(options) { }
    }
}
