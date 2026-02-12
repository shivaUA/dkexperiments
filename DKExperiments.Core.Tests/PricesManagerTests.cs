using XMPricesAggregator.Core.Calculations;
using XMPricesAggregator.Core.Services;
using XMPricesAggregator.DB.Repositories;
using XMPricesAggregator.Tests.Base;

namespace XMPricesAggregator.Core.Tests;

[Collection("Database collection")]
public class PricesManagerTests(XMDatabaseFixture fixture)
{
	[Fact]
	public async Task TestAll()
	{
		var repo = new AggregatedPricesRepository(fixture.DBContext);
		var calc = new Calculator(fixture.Config);
		var srv = new PricesManager(repo, fixture.ServiceProvider, calc);

		var time = new DateTime(2025, 01, 01, 0, 0, 0, 0, 0, DateTimeKind.Utc);

		var singleRecord = await srv.LoadSingleAsync(Models.Markets.BTCUSD, time);

		Assert.True(singleRecord != null);
		Assert.True(singleRecord.Close > 0);

		var listOfRecords = await srv.LoadRangeAsync(Models.Markets.BTCUSD, time, time);

		Assert.True(listOfRecords.Count == 1);
	}
}