using DKExperiments.Core.Models;
using DKExperiments.DB.Models.Prices;
using DKExperiments.DB.Repositories;
using DKExperiments.Tests.Base;

namespace DKExperiments.DB.Tests;

[Collection("Database collection")]
public class AggregatedPricesRepositoryTests(DKDatabaseFixture fixture)
{
	[Fact]
	public async Task AggregatedPrices_Save()
	{
		var cancelToken = new CancellationToken();

		var repo = new AggregatedPricesRepository(fixture.DBContext);

		var rec = new AggregatedPrice
		{
			Timestamp = new DateTime(2025, 12, 23, 19, 0, 0, 0, 0, DateTimeKind.Utc),
			Market = (int)Markets.BTCUSD,
			Close = 10,
			Final = false
		};

		await repo.SaveAsync(rec, false, cancelToken);

		var count = fixture.DBContext.AggregatedPrices.Count();
		Assert.Equal(1, count);

		var item = fixture.DBContext.AggregatedPrices.First();
		Assert.Equal(10, item.Close);

		rec.Close = 15;
		rec.Final = true;
		await repo.SaveAsync(rec, false, cancelToken);

		item = fixture.DBContext.AggregatedPrices.First();

		Assert.Equal(15, item.Close);

		item.Close = 20;
		await Assert.ThrowsAsync<InvalidOperationException>(async () => await repo.SaveAsync(item, false, cancelToken));
	}

	[Fact]
	public async Task AggregatedPrices_Get()
	{
		var cancelToken = new CancellationToken();

		var repo = new AggregatedPricesRepository(fixture.DBContext);

		var time = new DateTime(2025, 12, 23, 18, 0, 0, 0, 0, DateTimeKind.Utc);
		var market = (int)Markets.BTCUSD;

		await repo.SaveAsync(new AggregatedPrice
		{
			Timestamp = time,
			Market = market,
			Close = 10,
			Final = false
		}, false, cancelToken);

		var item = await repo.GetAsync(new AggregatedPriceSearchModel(time, time, market), cancelToken);
		Assert.True(item != null);
		Assert.Equal(10, item.Close);

		item = await repo.GetAsync(new AggregatedPriceSearchModel(time, time, (int)Markets.Undefined), cancelToken);
		Assert.True(item == null);
	}

	[Fact]
	public async Task AggregatedPrices_List()
	{
		var cancelToken = new CancellationToken();

		var repo = new AggregatedPricesRepository(fixture.DBContext);

		var time = new DateTime(2025, 12, 23, 15, 0, 0, 0, 0, DateTimeKind.Utc);
		var market = (int)Markets.BTCUSD;

		await repo.SaveAsync(new AggregatedPrice
		{
			Timestamp = time,
			Market = market,
			Close = 10,
			Final = true
		}, false, cancelToken);

		var items = await repo.ListAsync(new AggregatedPriceSearchModel(time, null, market), cancelToken);
		Assert.True(items.Count > 0);
	}
}
