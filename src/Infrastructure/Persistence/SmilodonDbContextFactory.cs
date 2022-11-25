using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Smilodon.Infrastructure.Persistence;

/// <summary>
/// A design-time <see cref="DbContext"/> factory for running migrations.
/// </summary>
/// <remarks>
/// This factory is only run when preforming migrations or database updates using the EF Core CLI.
/// </remarks>
public class SmilodonDbContextFactory : IDesignTimeDbContextFactory<SmilodonDbContext>
{
    public SmilodonDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SmilodonDbContext>();
        
        // Configure local database with snake_case naming conventions
        optionsBuilder
            .UseNpgsql("Host=localhost;Database=smilodon;")
            .UseSnakeCaseNamingConvention();

        // Construct the design-time DB Context below.
        return new SmilodonDbContext(optionsBuilder.Options);
    }
}