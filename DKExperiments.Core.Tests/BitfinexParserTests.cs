using DKExperiments.Core.Services;
using DKExperiments.Tests.Base;

namespace DKExperiments.Core.Tests;

[Collection("Config collection")]
public class BitfinexParserTests(XMConfigFixture fixture)
{
	[Fact]
	public async Task Parse()
	{
		var srv = new BitfinexParser(fixture.Config);
		var time = new DateTime(2025, 01, 01, 0, 0, 0, 0, 0, DateTimeKind.Utc);

		var data = await srv.ParseAsync(Models.Markets.BTCUSD, time);

		Assert.True(data != null);
		Assert.True(data.Close > 0);
	}
}