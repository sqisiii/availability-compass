using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddRecurringDatesRequest;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

public class AddRecurringDateViewModel : BaseDialogCrudViewModel<RecurringDateViewModel>
{
    private readonly IMediator _mediator;


    public AddRecurringDateViewModel(IMediator mediator, INavigationService<IDialogViewModel> dialogNavigationService) : base(dialogNavigationService)
    {
        _mediator = mediator;
    }

    public override void LoadData(object obj)
    {
        if (obj is Guid calendarId)
        {
            SelectedItem.CalendarId = calendarId;
        }
    }

    protected override async Task<IProcessResult> ProcessDataAsync(CancellationToken ct)
    {
        return await _mediator.Send(new AddRecurringDatesToDbRequest(SelectedItem.CalendarId,
            SelectedItem.Description,
            DateOnly.Parse(SelectedItem.StartDateString ?? string.Empty),
            SelectedItem.Duration ?? 1,
            SelectedItem.RepetitionPeriod ?? 1,
            SelectedItem.NumberOfRepetitions ?? 1), ct);
    }
}