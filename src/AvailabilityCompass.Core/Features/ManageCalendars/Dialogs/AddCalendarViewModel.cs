using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

/// <summary>
/// View model for the add calendar dialog.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public partial class AddCalendarViewModel : BaseDialogCrudViewModel<CalendarViewModel>
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