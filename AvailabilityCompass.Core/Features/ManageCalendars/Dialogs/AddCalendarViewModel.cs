using System.ComponentModel.DataAnnotations;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

public partial class AddCalendarViewModel : ObservableValidator, IDialogViewModel
{
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;
    private readonly IMediator _mediator;

    [ObservableProperty]
    private bool _isOnly;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(SaveCalendarCommand))]
    [Required]
    private string _name = string.Empty;

    public AddCalendarViewModel(IMediator mediator, INavigationService<IDialogViewModel> dialogNavigationService)
    {
        _mediator = mediator;
        _dialogNavigationService = dialogNavigationService;
    }

    public bool IsActive { get; set; }
    public bool IsDialogOpen { get; set; }

    public Task LoadDataAsync()
    {
        return Task.CompletedTask;
    }

    private bool CanSave() => !string.IsNullOrEmpty(Name);

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task OnSaveCalendar()
    {
        var result = await _mediator.Send(new AddCalendarToDbRequest(Name, IsOnly));

        if (result.IsSuccess)
        {
            _dialogNavigationService.CloseView();
        }
    }
}