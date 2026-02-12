using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DKExperiments.DB.Models.Prices;

namespace DKExperiments.DB.Configurations.Prices;

internal class AggregatedPriceConfiguration : IEntityTypeConfiguration<AggregatedPrice>
{
	public void Configure(EntityTypeBuilder<AggregatedPrice> builder)
	{
		// Table name
		builder.ToTable("aggregated_prices");

		// Fields
		builder
			.Property(b => b.Timestamp)
			.IsRequired()
			.HasDefaultValueSql("current_timestamp at time zone 'utc'")
			.ValueGeneratedOnAdd();

		builder
			.Property(b => b.Market)
			.IsRequired()
			.HasDefaultValue(1);

		builder
			.Property(b => b.Final)
			.IsRequired()
			.HasDefaultValue(true);

		//builder
		//	.Property(b => b.Open)
		//	.IsRequired()
		//	.HasDefaultValue(0);

		//builder
		//	.Property(b => b.High)
		//	.IsRequired()
		//	.HasDefaultValue(0);

		//builder
		//	.Property(b => b.Low)
		//	.IsRequired()
		//	.HasDefaultValue(0);

		builder
			.Property(b => b.Close)
			.IsRequired()
			.HasDefaultValue(0);

		builder
			.Property(b => b.LastUpdated)
			.IsRequired()
			.HasDefaultValueSql("current_timestamp at time zone 'utc'");

		builder
			.Property<uint>("Version")
			.IsRowVersion();

		// Primary Key
		builder.HasKey(x => new { x.Timestamp, x.Market }).HasName("pk_aggregated_prices");
	}
}
