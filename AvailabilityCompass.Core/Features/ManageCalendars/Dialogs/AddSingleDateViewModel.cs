using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddSingleDateRequest;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

public class AddSingleDateViewModel : CrudViewModelBase<SingleDateViewModel>
{
    private readonly IMediator _mediator;

    public AddSingleDateViewModel(IMediator mediator, INavigationService<IDialogViewModel> dialogNavigationService) : base(dialogNavigationService)
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
        return await _mediator.Send(new AddSingleDateToDbRequest(SelectedItem.CalendarId,
            SelectedItem.Description,
            DateOnly.Parse(SelectedItem.DateString ?? string.Empty)), ct);
    }
}