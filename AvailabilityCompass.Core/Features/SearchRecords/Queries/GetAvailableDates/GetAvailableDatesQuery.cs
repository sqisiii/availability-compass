using MediatR;

namespace AvailabilityCompass.Core.Features.SearchRecords.Queries.GetAvailableDates;

public record GetAvailableDatesQuery(List<Guid> CalendarIds) : IRequest<GetAvailableDatesResponse>;