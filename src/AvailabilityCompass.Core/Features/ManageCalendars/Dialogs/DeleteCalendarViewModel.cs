using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteCalendarRequest;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

/// <summary>
/// View model for the delete calendar confirmation dialog.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class DeleteCalendarViewModel : BaseDialogCrudViewModel<CalendarViewModel>
{
    private readonly IMediator _mediator;

    public DeleteCalendarViewModel(IMediator mediator, INavigationService<IDialogViewModel> dialogNavigationService) : base(dialogNavigationService)
    {
        _mediator = mediator;
    }

    public override void LoadData(object obj)
    {
        if (obj is CalendarViewModel calendarViewModel)
        {
            //copy only properties that will be shown or used
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
        return await _mediator.Send(new DeleteCalendarFromDbRequest(SelectedItem.CalendarId), ct);
    }
}