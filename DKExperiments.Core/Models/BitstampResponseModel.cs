namespace DKExperiments.Core.Models;

public record BitstampResponseModel
{
	public string? Message { get; set; }

	public BitstampResponseData? Data { get; set; }

	public record BitstampResponseData
	{
		public required string Pair { get; set; }

		public required List<BitstampResponseOHLC> Ohlc { get; set; }

		public record BitstampResponseOHLC
		{
			public required long Timestamp { get; set; }
			public required decimal Open { get; set; }
			public required decimal High { get; set; }
			public required decimal Low { get; set; }
			public required decimal Close { get; set; }
			public required decimal Volume { get; set; }
		}
	}
}
