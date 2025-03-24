using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Shared;

/// <summary>
/// Provides extension methods for the <see cref="ObservableValidator"/> class.
/// </summary>
public static class ObservableValidatorExtensions
{
    /// <summary>
    /// Validates all properties of the specified <see cref="ObservableValidator"/> instance.
    /// </summary>
    /// <remarks>
    /// ObservableValidator defines ValidateAllProperties as protected so reflection is used to call it.
    /// </remarks>
    /// <param name="validator">The <see cref="ObservableValidator"/> instance to validate.</param>
    public static void ValidateAll(this ObservableValidator validator)
    {
        // Use reflection to call the protected method
        typeof(ObservableValidator)
            .GetMethod("ValidateAllProperties",
                BindingFlags.Instance |
                BindingFlags.NonPublic)
            ?
            .Invoke(validator, null);
    }
}