namespace DKExperiments.Core.Models;

public record ParserSettingsModel
{
	public string? Url { get; set; }
	public string? TimestampType { get; set; }
	public string? TimestampFormat { get; set; }
}

public record ParsersConfigModel
{
	public ParserSettingsModel? Bitfinex { get; set; }
	public ParserSettingsModel? Bitstamp { get; set; }
}
