namespace DKExperiments.Core.Extensions;

public static class DateTimeExtensionMethods
{
	/// <summary>
	/// Converts DateTime to a proper Unix time used in external APIs
	/// </summary>
	/// <param name="time">Date and hour</param>
	/// <param name="timestampType">Timestamp type. Only "unix-timestamp" available at the moment</param>
	/// <param name="timestampFormat">Timestamp format - "seconds" or "milliseconds"</param>
	/// <returns></returns>
	internal static string ToAPITimestamp(this DateTime time, string timestampType, string timestampFormat)
	{
		string? timestamp;

		if (timestampType.Equals("unix-timestamp", StringComparison.OrdinalIgnoreCase))
		{
			var offset = new DateTimeOffset(time);
			var stamp = timestampFormat.Equals("seconds", StringComparison.OrdinalIgnoreCase) ? offset.ToUnixTimeSeconds() : offset.ToUnixTimeMilliseconds();
			timestamp = stamp.ToString();
		}
		else
		{
			timestamp = time.ToString("yyyyMMddHHmm");
		}

		return timestamp;
	}

	/// <summary>
	/// Cut seconds, milliseconds and other small units from the date, leave only Date and hour.<br />
	/// Also specifies that the date is UTC.
	/// </summary>
	/// <param name="time">Date and time</param>
	/// <returns></returns>
	public static DateTime RoundToHours(this DateTime time) =>
		new (time.Year, time.Month, time.Day, time.Hour, 0, 0, 0, 0, DateTimeKind.Utc);
}
