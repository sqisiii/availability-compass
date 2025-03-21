using System.ComponentModel.DataAnnotations;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddSingleDateRequest;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using AvailabilityCompass.Core.Shared.ValidationAttributes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

public partial class AddSingleDateViewModel : ObservableValidator, IDialogViewModel
{
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;
    private readonly IMediator _mediator;

    [ObservableProperty]
    [DateValidation]
    [NotifyCanExecuteChangedFor(nameof(SaveSingleDateCommand))]
    [Required(ErrorMessage = "Date is required")]
    [NotifyDataErrorInfo]
    private string? _date;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(SaveSingleDateCommand))]
    [Required(ErrorMessage = "Description is required")]
    private string _description = string.Empty;


    public AddSingleDateViewModel(IMediator mediator, INavigationService<IDialogViewModel> dialogNavigationService)
    {
        _mediator = mediator;
        _dialogNavigationService = dialogNavigationService;
    }

    public Guid CalendarId { get; set; }
    public bool IsActive { get; set; }
    public bool IsDialogOpen { get; set; }


    public Task LoadDataAsync()
    {
        return Task.CompletedTask;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task OnSaveSingleDate(CancellationToken ct)
    {
        var result = await _mediator.Send(new AddSingleDateToDbRequest(CalendarId,
            Description,
            DateOnly.Parse(Date ?? string.Empty)), ct);
        if (result.isSuccess)
        {
            _dialogNavigationService.CloseView();
        }
    }

    private bool CanSave() => !HasErrors && !string.IsNullOrWhiteSpace(Description) && !string.IsNullOrWhiteSpace(Date);
}