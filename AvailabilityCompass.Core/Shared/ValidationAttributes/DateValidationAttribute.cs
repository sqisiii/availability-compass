using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AvailabilityCompass.Core.Shared.ValidationAttributes;

/// <summary>
/// Validates that a given date string matches one of the specified date formats.
/// </summary>
public class DateValidationAttribute : ValidationAttribute
{
    private readonly string[] _formats;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateValidationAttribute"/> class.
    /// </summary>
    /// <param name="formats">An array of date formats to validate against. If no formats are provided, "yyyy-MM-dd" is used by default.</param>
    public DateValidationAttribute(params string[] formats)
    {
        _formats = formats.Length > 0 ? formats : new[] { "yyyy-MM-dd" };
    }

    /// <summary>
    /// Validates the specified value with respect to the current validation attribute.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>An instance of <see cref="ValidationResult"/>.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        switch (value)
        {
            case null:
            case DateOnly:
            case string dateString when string.IsNullOrWhiteSpace(dateString):
                return ValidationResult.Success;
            case string dateString when DateOnly.TryParseExact(dateString, _formats,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _):
                return ValidationResult.Success;
            case string dateString:
                return new ValidationResult(ErrorMessage ?? $"Please enter a valid date: yyyy-MM-dd");
            default:
                return ValidationResult.Success;
        }
    }
}
