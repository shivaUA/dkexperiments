using DKExperiments.Core.Models;

namespace DKExperiments.Core.Services.Abstractions;

public interface IPricesManager
{
	Task<PriceInfo?> LoadSingleAsync(Markets market, DateTime time, bool timeout, CancellationToken cancellationToken);

	Task<List<PriceInfo>> LoadRangeAsync(Markets market, DateTime start, DateTime end, CancellationToken cancellationToken);
}
