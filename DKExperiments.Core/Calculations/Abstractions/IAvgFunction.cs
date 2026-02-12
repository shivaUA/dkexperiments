namespace DKExperiments.Core.Calculations.Abstractions;

public interface IAvgFunction
{
	/// <summary>
	/// Average value calculation
	/// </summary>
	/// <param name="prices">List of prices for calculations</param>
	/// <returns></returns>
	decimal Calculate(List<decimal> prices);
}
