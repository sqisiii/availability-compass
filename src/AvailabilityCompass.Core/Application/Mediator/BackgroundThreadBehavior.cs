using MediatR;

namespace AvailabilityCompass.Core.Application.Mediator;

/// <summary>
/// Runs request handlers on the thread pool when the caller is on a UI thread.
/// Microsoft.Data.Sqlite executes its async API synchronously, so without this hop
/// every handler's DB work (queries, transactions, commits) would block the UI thread.
/// </summary>
public sealed class BackgroundThreadBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        return SynchronizationContext.Current is null
            ? next()
            : Task.Run(() => next(), cancellationToken);
    }
}