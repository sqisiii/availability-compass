using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateCalendarRequest;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

/// <summary>
/// View model for the update calendar dialog.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class UpdateCalendarViewModel : BaseDialogCrudViewModel<CalendarViewModel>
{
    private readonly IMediator _mediator;

    public UpdateCalendarViewModel(IMediator mediator, INavigationService<IDialogViewModel> dialogNavigationService) : base(dialogNavigationService)
    {
        _mediator = mediator;
    }

    public override void LoadData(object obj)
    {
        if (obj is CalendarViewModel calendarViewModel)
        {
            //copy only properties that can be edited
            SelectedItem = new CalendarViewModel
            {
                CalendarId = calendarViewModel.CalendarId,
                Name = calendarViewModel.Name,
                IsOnly = calendarViewModel.IsOnly
            };
        }
    }


    protected override async Task<IProcessResult> ProcessDataAsync(CancellationToken ct)
    {
        return await _mediator.Send(new UpdateCalendarInDbRequest(SelectedItem.CalendarId, SelectedItem.Name, SelectedItem.IsOnly), ct);
    }
}