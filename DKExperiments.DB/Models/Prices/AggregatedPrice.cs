namespace DKExperiments.DB.Models.Prices;

/// <summary>
/// Aggregated price for a specific market at a specific date and hour
/// </summary>
public class AggregatedPrice
{
	/// <summary>
	/// Data info date and hour - UTC timestamp<br />
	/// Makes a primary key together with <b>Market</b>
	/// </summary>
	public DateTime Timestamp { get; set; }

	/// <summary>
	/// Market identifier from Markets enum<br />
	/// Makes a primary key together with <b>Timestamp</b>
	/// </summary>
	public int Market { get; set; } = 1;

	/// <summary>
	/// Indicates if data is final and cannot be updated anymore<br />
	/// <b>True</b> for all past dates/hours<br />
	/// <b>False</b> for current hour, so it will be updated later
	/// </summary>
	public bool Final { get; set; } = true;

	// Uncomment all the prices and update table configuration, repository and database if needed

	///// <summary>
	///// Open price for a specified hour
	///// </summary>
	//public decimal Open { get; set; } = 0;

	///// <summary>
	///// Highest price for a specified hour
	///// </summary>
	//public decimal High { get; set; } = 0;

	///// <summary>
	///// Lowest price for a specified hour
	///// </summary>
	//public decimal Low { get; set; } = 0;

	/// <summary>
	/// Close price for a specified hour
	/// </summary>
	public decimal Close { get; set; } = 0;

	/// <summary>
	/// UTC timestamp<br />
	/// Shows the date and time of the last update<br />
	/// Needed for loading fresh data during the current hour
	/// </summary>
	public DateTime LastUpdated { get; set; }
}
