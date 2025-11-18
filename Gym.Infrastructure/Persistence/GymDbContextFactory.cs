using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Gym.Infrastructure.Persistence;

public class GymDbContextFactory : IDesignTimeDbContextFactory<GymDbContext>
{
    public GymDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GymDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=GymDb;User Id=sa;Password=YourStrong!Passw0rd!;Encrypt=false",
            sql => { }
        );

        return new GymDbContext(optionsBuilder.Options);
    }
}