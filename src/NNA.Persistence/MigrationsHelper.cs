using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NNA.Persistence;

public sealed class MigrationsHelper: IDesignTimeDbContextFactory<NnaContext> {
    
    public NnaContext CreateDbContext(string[] args) {
        
        var optionsBuilder = new DbContextOptionsBuilder<NnaContext>();
        optionsBuilder.UseSqlServer(args[0], migrationOptions => {
            migrationOptions.MigrationsAssembly("NNA.Persistence");
        });

        return new NnaContext(optionsBuilder.Options);
    }
}
