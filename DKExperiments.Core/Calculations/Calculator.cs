using Microsoft.Extensions.Configuration;
using DKExperiments.Core.Calculations.Abstractions;

namespace DKExperiments.Core.Calculations;

public class Calculator(IConfiguration config) : ICalculator
{
	public decimal CalculateAvg(List<decimal> prices)
	{
		var fomula = config.GetSection("PriceAPIs:AVGCalculationFormula").Value ?? "basic-avg";

		return fomula switch
		{
			"basic-avg" => new BasicAvgFunction().Calculate(prices),
			_ => throw new NotImplementedException($"Average calculation formula \"{fomula}\" does not have an implementation"),
		};
	}
}
