using System.ComponentModel.DataAnnotations;

namespace DKExperiments.API.Validation;

/// <summary>
/// Custom validation attribute for comparing two dates.<br />
/// The current date <b>must</b> be bigger than the one it's compared to
/// </summary>
public class BiggerDateOnlyAttribute(string OtherProperty) : ValidationAttribute()
{
	public override bool RequiresValidationContext => true;

	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value == null)
		{
			return ValidationResult.Success;
		}

		if (value is not DateTime)
		{
			return new ValidationResult($"Property \"{validationContext.DisplayName}\" must be a DateTime field");
		}

		var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
		if (otherPropertyInfo == null)
		{
			return new ValidationResult($"Unknown property \"{OtherProperty}\"");
		}

		var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
		if (otherPropertyValue == null)
		{
			return ValidationResult.Success;
		}

		if (otherPropertyValue is not DateTime)
		{
			return new ValidationResult($"Property \"{OtherProperty}\" must be a DateTime field");
		}

		return (DateTime)otherPropertyValue < (DateTime)value
			? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
			: ValidationResult.Success;
	}
}
