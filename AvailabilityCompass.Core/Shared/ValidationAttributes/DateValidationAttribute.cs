using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AvailabilityCompass.Core.Shared.ValidationAttributes;

public class DateValidationAttribute : ValidationAttribute
{
    private readonly string[] _formats;

    public DateValidationAttribute(params string[] formats)
    {
        _formats = formats.Length > 0 ? formats : ["yyyy-MM-dd"];
    }

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