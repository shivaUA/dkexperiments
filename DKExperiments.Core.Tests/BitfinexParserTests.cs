using DKExperiments.Core.Models;
using DKExperiments.Core.Services;
using DKExperiments.Tests.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DKExperiments.Core.Tests;

[Collection("Config collection")]
public class BitfinexParserTests(DKConfigFixture fixture)
{
	[Fact]
	public async Task Parse()
	{
		var ct = new CancellationToken();

		var httpFactory = new DefaultHttpClientFactory();

		var parserConfig = fixture.Config.GetSection("PriceAPIs");
		var configModel = parserConfig.Get<ParsersConfigModel>()!;
		var configOption = Options.Create(configModel);

		var srv = new BitfinexParser(configOption, httpFactory);
		var time = new DateTime(2025, 01, 01, 0, 0, 0, 0, 0, DateTimeKind.Utc);

		var data = await srv.ParseAsync(Models.Markets.BTCUSD, time, ct);

		Assert.True(data != null);
		Assert.True(data.Close > 0);
	}
}