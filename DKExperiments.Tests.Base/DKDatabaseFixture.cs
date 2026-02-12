using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DKExperiments.Core.Dependency;
using DKExperiments.DB;
using DKExperiments.DB.Dependency;

namespace DKExperiments.Tests.Base;

public class DKDatabaseFixture : IDisposable
{
	public ServiceProvider ServiceProvider { get; }
	public DKDBContext DBContext { get; }
	public IConfiguration Config { get; }

	public DKDatabaseFixture()
	{
		var cb = new ConfigurationBuilder();
		cb.AddJsonFile("appsettings.json");
		Config = cb.Build();

		var sc = new ServiceCollection()
			.AddSingleton(Config)
			.AddDBServices(Config)
			.AddCoreServices();

		ServiceProvider = sc.BuildServiceProvider();

		DBContext = ServiceProvider.GetRequiredService<DKDBContext>();

		DKDBContext.Migrate(ServiceProvider);
	}

	public void Dispose()
	{
		var tableNames = DBContext.Model.GetEntityTypes()
			.Select(t => t.GetTableName())
			.Distinct()
			.ToList();

		foreach (var tableName in tableNames)
		{
			DBContext.Database.ExecuteSqlRaw($"DELETE FROM \"{tableName}\";");
		}

		ServiceProvider.Dispose();

		GC.SuppressFinalize(this);
	}
}
