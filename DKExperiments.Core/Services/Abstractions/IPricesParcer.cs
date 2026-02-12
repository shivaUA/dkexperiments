using DKExperiments.Core.Models;

namespace DKExperiments.Core.Services.Abstractions;

public interface IPricesParcer
{
	/// <summary>
	/// Parse prices data from an External API
	/// </summary>
	/// <param name="market">Market for which you want to load prices</param>
	/// <param name="time">Date and hour</param>
	/// <returns></returns>
	Task<ParcerOHLCResponse?> ParseAsync(Markets market, DateTime time, CancellationToken cancellationToken);
}
