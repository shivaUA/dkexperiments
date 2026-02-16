using DKExperiments.DB.Models.Prices;
using DKExperiments.DB.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DKExperiments.DB;

public class DKDBContext(DbContextOptions<DKDBContext> options) : DbContext(options)
{
	public DbSet<AggregatedPrice> AggregatedPrices { get; set; }
	public DbSet<User> Users { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder) =>
		modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

	public static void Migrate(IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<DKDBContext>();
		dbContext.Database.Migrate();
	}
}
