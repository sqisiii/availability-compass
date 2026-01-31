using System.ComponentModel.DataAnnotations;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Validates that EditorFrequency is at least EditorDuration + 1 when recurring is enabled.
/// </summary>
public class FrequencyValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.ObjectInstance is not DateEntryEditorController controller
            || !controller.EditorIsRecurring)
        {
            return ValidationResult.Success;
        }

        if (value is not int frequency)
        {
            return new ValidationResult("Required");
        }

        var minFrequency = controller.EditorDuration + 1;
        return frequency < minFrequency
            ? new ValidationResult($"Must be at least {minFrequency}")
            : ValidationResult.Success;
    }
}