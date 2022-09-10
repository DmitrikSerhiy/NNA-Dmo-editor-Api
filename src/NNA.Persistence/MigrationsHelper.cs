using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NNA.Persistence;

public sealed class MigrationsHelper: IDesignTimeDbContextFactory<NnaContext> {

    private static string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=nna-local;"; // change for  prod db manually
    public NnaContext CreateDbContext(string[] args) {
        
        var optionsBuilder = new DbContextOptionsBuilder<NnaContext>();
        optionsBuilder.UseSqlServer(_connectionString, migrationOptions => {
            migrationOptions.MigrationsAssembly("NNA.Persistence");
        });

        return new NnaContext(optionsBuilder.Options);
    }
}
