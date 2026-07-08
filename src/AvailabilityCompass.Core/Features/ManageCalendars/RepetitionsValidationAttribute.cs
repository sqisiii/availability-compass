using System.ComponentModel.DataAnnotations;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Validates that EditorRepetitions is a non-negative integer when recurring is enabled.
/// </summary>
public class RepetitionsValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.ObjectInstance is not DateEntryEditorController controller
            || !controller.EditorIsRecurring)
        {
            return ValidationResult.Success;
        }

        if (value is not int repetitions)
        {
            return new ValidationResult("Required");
        }

        return repetitions switch
        {
            < 0 => new ValidationResult("Must be 0 or greater"),
            > DateEntryLimits.MaxRepetitions =>
                new ValidationResult($"Must be {DateEntryLimits.MaxRepetitions} or fewer"),
            _ => ValidationResult.Success
        };
    }
}
