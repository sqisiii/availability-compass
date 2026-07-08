namespace AvailabilityCompass.MauiClient.Shared.Navigation;

/// <summary>
/// Serializes Shell route pushes. Independent commands (two toolbar buttons, the
/// startup auto-push) each guard only their own re-entry, so without a shared gate
/// overlapping GoToAsync calls stack duplicate pages.
/// </summary>
public static class NavigationGate
{
    private static bool _navigationInFlight;

    public static async Task GoToAsync(string route)
    {
        if (_navigationInFlight)
        {
            return;
        }

        _navigationInFlight = true;
        try
        {
            await Shell.Current.GoToAsync(route);
        }
        finally
        {
            _navigationInFlight = false;
        }
    }
}