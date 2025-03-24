using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteRecurringDateRequest;

public record DeleteRecurringDateFromDbRequest(Guid CalendarId, Guid RecurringDateId) : IRequest<DeleteRecurringDateFromDbResponse>;