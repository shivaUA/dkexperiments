using DKExperiments.Core.Calculations.Abstractions;

namespace DKExperiments.Core.Calculations;

public class BasicAvgFunction : IAvgFunction
{
	public decimal Calculate(List<decimal> prices) => prices.Average();
}
