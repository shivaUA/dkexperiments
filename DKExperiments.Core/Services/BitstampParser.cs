using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using DKExperiments.Core.Extensions;
using DKExperiments.Core.Models;
using DKExperiments.Core.Services.Abstractions;

namespace DKExperiments.Core.Services;

public class BitstampParser(IOptions<ParsersConfigModel> parsersConfig, IHttpClientFactory httpClientFactory) : IPricesParcer
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

		if (string.IsNullOrWhiteSpace(config.Bitstamp?.Url))
		{
			// Write logs instead of throwing an exception
			// throw new NullReferenceException("Bitstamp settings are incorrect, please contact developer or system administrator");

			return null;
		}

		var timestampType = config.Bitstamp.TimestampType ?? "unix-timestamp";
		var timestampFormat = config.Bitstamp.TimestampFormat ?? "seconds";

		var timestamp = time.ToAPITimestamp(timestampType, timestampFormat);
		var marketSymbol = Enum.GetName(typeof(Markets), market)!.ToLower();

		var url = string.Format(config.Bitstamp.Url, marketSymbol, timestamp);

		var client = httpClientFactory.CreateClient("coreHttpClient");

		var response = await client.GetAsync(url, cancellationToken);

		if (response != null && !cancellationToken.IsCancellationRequested)
		{
			var content = await response.Content.ReadAsStringAsync(cancellationToken);
			var ohlcData = JsonConvert.DeserializeObject<BitstampResponseModel>(content) ?? new BitstampResponseModel();

			if (ohlcData.Data != null && ohlcData.Data.Ohlc.Count > 0)
			{
				var item = ohlcData.Data.Ohlc.First();

				return new ParcerOHLCResponse
				{
					// Uncomment all prices in case they are needed
					//Open = item.Open,
					//High = item.High,
					//Low = item.Low,
					Close = item.Close
				};
			}
		}

		return null;
	}
}
