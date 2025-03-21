using System.ComponentModel.DataAnnotations;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddRecurringDatesRequest;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using AvailabilityCompass.Core.Shared.ValidationAttributes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

public partial class AddRecurringDateViewModel : ObservableValidator, IDialogViewModel
{
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;
    private readonly IMediator _mediator;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(SaveRecurringDateCommand))]
    [Required(ErrorMessage = "Description is required")]
    private string _description = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Duration is required")]
    [NotifyCanExecuteChangedFor(nameof(SaveRecurringDateCommand))]
    [Range(1, 365)]
    private int? _duration;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Number of repetitions is required")]
    [NotifyCanExecuteChangedFor(nameof(SaveRecurringDateCommand))]
    [Range(1, int.MaxValue)]
    private int? _numberOfRepetitions;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Repetition period is required")]
    [NotifyCanExecuteChangedFor(nameof(SaveRecurringDateCommand))]
    [Range(1, 365)]
    private int? _repetitionPeriod;

    [ObservableProperty]
    [DateValidation]
    [Required(ErrorMessage = "Start date is required")]
    [NotifyCanExecuteChangedFor(nameof(SaveRecurringDateCommand))]
    [NotifyDataErrorInfo]
    private string? _startDate;


    public AddRecurringDateViewModel(IMediator mediator, INavigationService<IDialogViewModel> dialogNavigationService)
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
    private async Task OnSaveRecurringDate()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            return;
        }

        var result = await _mediator.Send(new AddRecurringDatesToDbRequest(CalendarId,
            Description,
            DateOnly.Parse(StartDate ?? string.Empty),
            Duration ?? 1,
            RepetitionPeriod ?? 1,
            NumberOfRepetitions ?? 1));
        if (result.IsSuccess)
        {
            _dialogNavigationService.CloseView();
        }
    }

    private bool CanSave()
    {
        return !HasErrors;
    }
}