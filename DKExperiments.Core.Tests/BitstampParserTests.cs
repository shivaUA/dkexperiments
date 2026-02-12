using XMPricesAggregator.Core.Services;
using XMPricesAggregator.Tests.Base;

namespace XMPricesAggregator.Core.Tests;

[Collection("Config collection")]
public class BitstampParserTests(XMConfigFixture fixture)
{
	[Fact]
	public async Task Parse()
	{
		var srv = new BitstampParser(fixture.Config);
		var time = new DateTime(2025, 01, 01, 0, 0, 0, 0, 0, DateTimeKind.Utc);

		var data = await srv.ParseAsync(Models.Markets.BTCUSD, time);

		Assert.True(data != null);
		Assert.True(data.Close > 0);
	}
}