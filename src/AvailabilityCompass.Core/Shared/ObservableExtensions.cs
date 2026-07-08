using System.Reactive.Linq;
using Serilog;

namespace AvailabilityCompass.Core.Shared;

public static class ObservableExtensions
{
    /// <summary>
    /// Runs an async handler per notification, logging and swallowing its exceptions.
    /// Without this, one handler exception terminates the whole Rx subscription and the
    /// UI silently stops reacting to events for the rest of the session.
    /// </summary>
    public static IObservable<System.Reactive.Unit> SelectManySafe<T>(
        this IObservable<T> source,
        Func<T, CancellationToken, Task> handler,
        string errorMessage)
    {
        return source.SelectMany(evt => Observable.FromAsync(ct => handler(evt, ct))
            .Catch((Exception ex) =>
            {
                Log.Error(ex, errorMessage);
                return Observable.Empty<System.Reactive.Unit>();
            }));
    }
}
