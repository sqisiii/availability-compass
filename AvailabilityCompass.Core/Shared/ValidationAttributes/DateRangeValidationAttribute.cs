using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AvailabilityCompass.Core.Shared.ValidationAttributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class DateRangeValidationAttribute : ValidationAttribute
{
    private const string DateFormat = "yyyy-MM-dd";
    private readonly bool _isStartDate;
    private readonly string _otherDatePropertyName;

    public DateRangeValidationAttribute(string otherDatePropertyName, bool isStartDate = false)
    {
        _otherDatePropertyName = otherDatePropertyName;
        _isStartDate = isStartDate;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // If value is null or empty, validation passes
        if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
        {
            return ValidationResult.Success;
        }

        // Get the other date property value using reflection
        var otherDateProperty = validationContext.ObjectType.GetProperty(_otherDatePropertyName);
        if (otherDateProperty == null)
        {
            return new ValidationResult($"Unknown property: {_otherDatePropertyName}");
        }

        var otherDateValue = otherDateProperty.GetValue(validationContext.ObjectInstance);

        // If other date is null but this date has a value, that's valid
        if (otherDateValue == null || (otherDateValue is string otherStr && string.IsNullOrWhiteSpace(otherStr)))
        {
            return ValidationResult.Success;
        }

        // Parse string dates
        if (value is not string thisDateString || otherDateValue is not string otherDateString)
        {
            return new ValidationResult("Date comparison failed. Both properties must be string type with date format yyyy-MM-dd.");
        }

        // Try to parse both dates
        var thisDateParsed = DateOnly.TryParseExact(
            thisDateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var thisDate);

        var otherDateParsed = DateOnly.TryParseExact(
            otherDateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var otherDate);

        // If either date is invalid, validation passes (other validators will catch format issues)
        if (!thisDateParsed || !otherDateParsed)
        {
            return ValidationResult.Success;
        }

        // For start date validation: this date must be before or equal to other date
        // For end date validation: this date must be after or equal to other date
        var isValid = _isStartDate ? thisDate <= otherDate : otherDate <= thisDate;

        if (isValid)
        {
            return ValidationResult.Success;
        }

        var errorMessage = _isStartDate
            ? ErrorMessage ?? "Start date must be before or on end date"
            : ErrorMessage ?? "End date must be after or on start date";

        return new ValidationResult(errorMessage);
    }
}