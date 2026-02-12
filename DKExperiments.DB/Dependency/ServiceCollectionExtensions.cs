using DKExperiments.DB.Repositories;
using DKExperiments.DB.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DKExperiments.DB.Dependency;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDBServices(this IServiceCollection services, IConfiguration config)
	{
		// Database connection
		services.AddDbContext<DKDBContext>(options => options.UseNpgsql(config.GetConnectionString("DKEConnectionString")));

		// Repositories
		services.AddScoped<IAggregatedPricesRepository, AggregatedPricesRepository>();

		return services;
	}
}
