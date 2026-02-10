using AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;
using AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendarsWithEntries;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetAvailableDates;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetAvailableDates;

/// <summary>
/// Handles requests to get available dates for a set of calendars.
/// </summary>
/// <remarks>
/// This handler retrieves calendar data from the database and calculates reserved dates
/// based on date entries defined in the calendars.
/// Request and Response are defined in the different feature <see cref="GetAvailableDatesQuery"/> and <see cref="GetAvailableDatesResponse"/> classes.
/// </remarks>
public class GetAvailableDatesHandler : IRequestHandler<GetAvailableDatesQuery, GetAvailableDatesResponse>
{
    private readonly IMediator _mediator;
    private readonly IReservedDatesCalculator _reservedDatesCalculator;

    public GetAvailableDatesHandler(
        IReservedDatesCalculator reservedDatesCalculator,
        IMediator mediator)
    {
        _reservedDatesCalculator = reservedDatesCalculator;
        _mediator = mediator;
    }

    public async Task<GetAvailableDatesResponse> Handle(GetAvailableDatesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var filteredCalendars = await GetCalendars(request.CalendarIds, cancellationToken);

            var reservedDates = _reservedDatesCalculator.GetReservedDates(filteredCalendars);

            return new GetAvailableDatesResponse(true, reservedDates);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error calculating available dates: {ErrorMessage}", ex.Message);
            return new GetAvailableDatesResponse(false, []);
        }
    }

    private async Task<List<CalendarDto>> GetCalendars(List<Guid> calendarIds, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _mediator.Send(new GetCalendarsWithEntriesQuery(calendarIds), cancellationToken);
            return response.IsSuccess ? response.Calendars : [];
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting selected calendars from database");
            return [];
        }
    }
}