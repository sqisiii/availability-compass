using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateSingleDateRequest;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

public class UpdateSingleDateViewModel : BaseDialogCrudViewModel<SingleDateViewModel>
{
    private readonly IMediator _mediator;

    public UpdateSingleDateViewModel(IMediator mediator, INavigationService<IDialogViewModel> dialogNavigationService) : base(dialogNavigationService)
    {
        _mediator = mediator;
    }

    public override void LoadData(object obj)
    {
        if (obj is not SingleDateViewModel calendarId)
        {
            return;
        }

        SelectedItem.CalendarId = calendarId.CalendarId;
        SelectedItem.Description = calendarId.Description;
        SelectedItem.Date = calendarId.Date;
        SelectedItem.SingleDateId = calendarId.SingleDateId;
    }


    protected override async Task<IProcessResult> ProcessDataAsync(CancellationToken ct)
    {
        if (SelectedItem.Date is null)
        {
            return new FailedProcessResult();
        }

        return await _mediator.Send(new UpdateSingleDateInDbRequest(SelectedItem.CalendarId, SelectedItem.Description, SelectedItem.Date.Value, SelectedItem.SingleDateId), ct);
    }
}