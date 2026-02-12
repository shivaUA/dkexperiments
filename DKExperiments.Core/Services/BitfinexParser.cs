using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using DKExperiments.Core.Extensions;
using DKExperiments.Core.Models;
using DKExperiments.Core.Services.Abstractions;

namespace DKExperiments.Core.Services;

public class BitfinexParser(IOptions<ParsersConfigModel> parsersConfig, IHttpClientFactory httpClientFactory) : IPricesParcer
{
	public async Task<ParcerOHLCResponse?> ParseAsync(Markets market, DateTime time, CancellationToken cancellationToken)
	{
		if (market == Markets.Undefined)
		{
			// Write logs instead of throwing an exception
			// throw new ArgumentException("Market is not specified. Please specify market to load data for");

			return null;
		}

		var config = parsersConfig.Value;

		if (string.IsNullOrWhiteSpace(config.Bitfinex?.Url))
		{
			// Write logs instead of throwing an exception
			// throw new NullReferenceException("Bitfinex settings are incorrect, please contact developer or system administrator");

			return null;
		}

		var timestampType = config.Bitfinex.TimestampType ?? "unix-timestamp";
		var timestampFormat = config.Bitfinex.TimestampFormat ?? "milliseconds";

		var timestamp = time.ToAPITimestamp(timestampType, timestampFormat);
		var marketSymbol = Enum.GetName(typeof(Markets), market)!;

		var url = string.Format(config.Bitfinex.Url, marketSymbol, timestamp);

		var client = httpClientFactory.CreateClient("coreHttpClient");

		var response = await client.GetAsync(url, cancellationToken);

		if (response != null && !cancellationToken.IsCancellationRequested)
		{
			var content = await response.Content.ReadAsStringAsync(cancellationToken);
			var data = JsonConvert.DeserializeObject<List<List<decimal>>>(content) ?? [];

			if (data.Count > 0 && data.First().Count > 0)
			{
				var item = data.First();

				return new ParcerOHLCResponse
				{
					// Uncomment all prices in case they are needed
					//Open = item[1],
					//High = item[3],
					//Low = item[4],
					Close = item[2]
				};
			}
		}

		return null;
	}
}
