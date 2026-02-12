namespace DKExperiments.Core.Models;

public class PriceInfo
{
	public DateTime Timestamp { get; set; }
	public decimal Close { get; set; } = 0;

	// Uncomment all prices in case they are needed
	//public decimal Open { get; set; } = 0;
	//public decimal High { get; set; } = 0;
	//public decimal Low { get; set; } = 0;
}
