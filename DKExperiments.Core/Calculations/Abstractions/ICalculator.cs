namespace DKExperiments.Core.Calculations.Abstractions;

public interface ICalculator
{
	/// <summary>
	/// Average value calculation<br />
	/// At the moment only a basic AVG calculation available but the functionality can be expanded in the future
	/// </summary>
	/// <param name="prices">List of prices for calculations</param>
	/// <returns></returns>
	decimal CalculateAvg(List<decimal> prices);
}
