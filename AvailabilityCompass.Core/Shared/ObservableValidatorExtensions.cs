using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Shared;

public static class ObservableValidatorExtensions
{
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