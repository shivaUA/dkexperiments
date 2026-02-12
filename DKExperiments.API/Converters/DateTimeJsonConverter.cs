using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DKExperiments.API.Converters;

public sealed class DateTimeJsonConverter : JsonConverter<DateTime>
{
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var res = DateTime.TryParseExact(reader.GetString() ?? "", "yyyy-MM-dd HH", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
			? dt
			: DateTime.MinValue;

		return new DateTime(res.Year, res.Month, res.Day, res.Hour, 0, 0, 0, 0, DateTimeKind.Utc);
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
		writer.WriteStringValue(value.ToString("yyyy-MM-dd HH", CultureInfo.InvariantCulture));
}
