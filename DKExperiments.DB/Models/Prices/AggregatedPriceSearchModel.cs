namespace DKExperiments.DB.Models.Prices;

/// <summary>
/// A simple model for performing a search in the aggregated prices table
/// </summary>
public record AggregatedPriceSearchModel(DateTime? Start, DateTime? End, int? Market);
