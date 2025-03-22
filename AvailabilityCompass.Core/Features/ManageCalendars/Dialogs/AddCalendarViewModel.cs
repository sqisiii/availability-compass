using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

public partial class AddCalendarViewModel : DialogBaseCrudViewModel<CalendarViewModel>
{
    private readonly IMediator _mediator;

    public AddCalendarViewModel(IMediator mediator, INavigationService<IDialogViewModel> dialogNavigationService) : base(dialogNavigationService)
    {
        _mediator = mediator;
    }

    protected override async Task<IProcessResult> ProcessDataAsync(CancellationToken ct)
    {
        return await _mediator.Send(new AddCalendarToDbRequest(SelectedItem.Name, SelectedItem.IsOnly), ct);
    }
}