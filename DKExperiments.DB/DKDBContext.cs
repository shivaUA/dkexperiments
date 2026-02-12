using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DKExperiments.DB.Models.Prices;

namespace DKExperiments.DB;

public class DKDBContext(DbContextOptions<DKDBContext> options) : DbContext(options)
{
	public DbSet<AggregatedPrice> AggregatedPrices { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder) =>
		modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

	public static void Migrate(IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<DKDBContext>();
		dbContext.Database.Migrate();
	}
}
