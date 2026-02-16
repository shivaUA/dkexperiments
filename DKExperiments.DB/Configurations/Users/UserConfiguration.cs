using DKExperiments.DB.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DKExperiments.DB.Configurations.Users;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("users");

		builder
			.Property(b => b.Id)
			.IsRequired()
			.HasDefaultValueSql("gen_random_uuid()");

		builder
			.Property(b => b.Username)
			.IsRequired()
			.HasMaxLength(100);

		builder
			.Property(b => b.Email)
			.IsRequired()
			.HasMaxLength(200);

		builder
			.Property(b => b.PasswordHash)
			.IsRequired();

		builder
			.Property(b => b.FirstName)
			.HasMaxLength(100);

		builder
			.Property(b => b.LastName)
			.HasMaxLength(100);

		builder
			.Property(b => b.IsActive)
			.IsRequired()
			.HasDefaultValue(true);

		builder
			.Property(b => b.Role)
			.HasMaxLength(50);

		builder
			.Property(b => b.CreatedAt)
			.IsRequired()
			.HasDefaultValueSql("current_timestamp at time zone 'utc'");

		builder
			.Property(b => b.UpdatedAt)
			.IsRequired()
			.HasDefaultValueSql("current_timestamp at time zone 'utc'");

		builder
			.Property<uint>("Version")
			.IsRowVersion();

		builder.HasKey(x => x.Id).HasName("pk_users");
	}
}
