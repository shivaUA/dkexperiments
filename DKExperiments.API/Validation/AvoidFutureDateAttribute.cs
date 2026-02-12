using System.ComponentModel.DataAnnotations;
using DKExperiments.Core.Extensions;

namespace DKExperiments.API.Validation;

/// <summary>
/// Custom validation attribute for avoiding future dates
/// </summary>
public class AvoidFutureDateAttribute : ValidationAttribute
{
	public override bool RequiresValidationContext => false;

	public override bool IsValid(object? value) => value is DateTime dateTime && dateTime > DateTime.UtcNow.RoundToHours();
}
