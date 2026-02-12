namespace DKExperiments.Core.Models;

public record ParserSettingsModel(string? Url, string? TimestampType, string? TimestampFormat);

public record ParsersConfigModel(ParserSettingsModel? Bitfinex, ParserSettingsModel? Bitstamp);
