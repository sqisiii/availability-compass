using MediatR;

namespace AvailabilityCompass.Core.Features.SearchRecords.Queries.GetAvailableDates;

/// <summary>
/// MediatR query to retrieve available dates based on selected calendar reservations.
/// </summary>
public record GetAvailableDatesQuery(List<Guid> CalendarIds) : IRequest<GetAvailableDatesResponse>;