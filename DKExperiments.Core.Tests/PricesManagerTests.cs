using DKExperiments.Core.Calculations;
using DKExperiments.Core.Services;
using DKExperiments.DB.Repositories;
using DKExperiments.Tests.Base;

namespace DKExperiments.Core.Tests;

[Collection("Database collection")]
public class PricesManagerTests(DKDatabaseFixture fixture)
{
	[Fact]
	public async Task TestAll()
	{
		var ct = new CancellationToken();

		var repo = new AggregatedPricesRepository(fixture.DBContext);
		var calc = new Calculator(fixture.Config);
		var srv = new PricesManager(repo, fixture.ServiceProvider, calc);

		var time = new DateTime(2025, 01, 01, 0, 0, 0, 0, 0, DateTimeKind.Utc);

		var singleRecord = await srv.LoadSingleAsync(Models.Markets.BTCUSD, time, false, ct);

		Assert.True(singleRecord != null);
		Assert.True(singleRecord.Close > 0);

		var listOfRecords = await srv.LoadRangeAsync(Models.Markets.BTCUSD, time, time, ct);

		Assert.True(listOfRecords.Count == 1);
	}
}