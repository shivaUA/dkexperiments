using Microsoft.Extensions.DependencyInjection;
using DKExperiments.Core.Calculations.Abstractions;
using DKExperiments.Core.Extensions;
using DKExperiments.Core.Models;
using DKExperiments.Core.Services.Abstractions;
using DKExperiments.DB.Models.Prices;
using DKExperiments.DB.Repositories.Abstractions;

namespace DKExperiments.Core.Services;

public class PricesManager(IAggregatedPricesRepository repo, IServiceProvider serviceProvider, ICalculator calculator) : IPricesManager
{
	/// <summary>
	/// Load single aggregated price (for one hour)
	/// </summary>
	/// <param name="market">Market/area for which you want to load prices</param>
	/// <param name="time">Date and hour</param>
	/// <returns></returns>
	public async Task<PriceInfo?> LoadSingleAsync(Markets market, DateTime time, bool timeout, CancellationToken cancellationToken)
	{
		var record = await repo.GetAsync(new AggregatedPriceSearchModel(time, time, (int)market), cancellationToken);

		if (record == null || (!record.Final && (DateTime.UtcNow - record.LastUpdated).TotalMinutes >= 5))
		{
			var services = serviceProvider.GetKeyedServices<IPricesParcer>("IPricesParcer") ?? [];
			var tasks = services.Select(x => x.ParseAsync(market, time, cancellationToken)).ToArray();

			var results = (await Task.WhenAll(tasks)).Where(x => x != null).ToList();
			if (results.Count > 0)
			{
				var avg = calculator.CalculateAvg([.. results.Select(x => x!.Close)]);

				record = await repo.SaveAsync(new AggregatedPrice
				{
					Timestamp = time,
					Market = (int)market,
					Final = DateTime.UtcNow.RoundToHours() > time,
					Close = avg
				}, timeout, cancellationToken);
			}
			else
			{
				throw new Exception("Error loading data from external resources. No data loaded");
			}
		}

		return new PriceInfo
		{
			Timestamp = time,
			Close = record!.Close
		};
	}

	/// <summary>
	/// Load list of aggregated prices for a specified period and market
	/// </summary>
	/// <param name="market">Market/area for which you want to load prices</param>
	/// <param name="start">Start date</param>
	/// <param name="end">End date</param>
	/// <returns></returns>
	public async Task<List<PriceInfo>> LoadRangeAsync(Markets market, DateTime start, DateTime end, CancellationToken cancellationToken)
	{
		var list = await repo.ListAsync(new AggregatedPriceSearchModel(start, end, (int)market), cancellationToken);

		return [..list
			.Select(x => new PriceInfo
			{
				Timestamp = x.Timestamp,
				Close = x.Close
			})];
	}
}
