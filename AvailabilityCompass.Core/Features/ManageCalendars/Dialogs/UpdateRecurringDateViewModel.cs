using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateRecurringDateRequest;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

public class UpdateRecurringDateViewModel : BaseDialogCrudViewModel<RecurringDateViewModel>
{
    private readonly IMediator _mediator;

    public UpdateRecurringDateViewModel(IMediator mediator, INavigationService<IDialogViewModel> dialogNavigationService) : base(dialogNavigationService)
    {
        _mediator = mediator;
    }

    public override void LoadData(object obj)
    {
        if (obj is not RecurringDateViewModel calendarId)
        {
            return;
        }

        SelectedItem.CalendarId = calendarId.CalendarId;
        SelectedItem.Description = calendarId.Description;
        SelectedItem.StartDate = calendarId.StartDate;
        SelectedItem.RepetitionPeriod = calendarId.RepetitionPeriod;
        SelectedItem.NumberOfRepetitions = calendarId.NumberOfRepetitions;
        SelectedItem.Duration = calendarId.Duration;
        SelectedItem.RecurringDateId = calendarId.RecurringDateId;
    }


    protected override async Task<IProcessResult> ProcessDataAsync(CancellationToken ct)
    {
        if (SelectedItem.StartDate is null || SelectedItem.RepetitionPeriod is null || SelectedItem.NumberOfRepetitions is null || SelectedItem.Duration is null)
        {
            return new FailedProcessResult();
        }

        return await _mediator.Send(new UpdateRecurringDateInDbRequest(SelectedItem.CalendarId,
                SelectedItem.RecurringDateId,
                SelectedItem.StartDate.Value,
                SelectedItem.Duration.Value,
                SelectedItem.Description,
                SelectedItem.RepetitionPeriod.Value,
                SelectedItem.NumberOfRepetitions.Value),
            ct);
    }
}