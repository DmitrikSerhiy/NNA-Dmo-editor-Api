﻿using System.IO;
using Microsoft.EntityFrameworkCore;
// ReSharper disable once RedundantUsingDirective
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Persistence;

namespace API.Helpers {
    // todo: create new migration strategy 
    public class MigrationHelper //: IDesignTimeDbContextFactory<NnaContext> {
    {
        public NnaContext CreateDbContext(string[] args) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../API"))
                .AddJsonFile($"appsettings.Local.json");
            var configuration = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<NnaContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                mySqlOptions => {
                    mySqlOptions.EnableRetryOnFailure(2);
                    mySqlOptions.MigrationsAssembly("Persistence");
                });
            return null; //new NnaContext(optionsBuilder.Options);
        }
    }
}
