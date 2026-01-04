using System.ComponentModel;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

public abstract partial class BaseDialogCrudViewModel<T> : ObservableValidator, IDialogViewModel, IDisposable where T : ObservableValidator, new()
{
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;

    [ObservableProperty]
    private T _selectedItem = new T();

    protected BaseDialogCrudViewModel(INavigationService<IDialogViewModel> dialogNavigationService)
    {
        _dialogNavigationService = dialogNavigationService;

        SelectedItem.ErrorsChanged += OnSelectedItemOnErrorsChanged;
    }

    public bool IsActive { get; set; }
    public bool IsDialogOpen { get; set; }

    public void Dispose()
    {
        SelectedItem.ErrorsChanged -= OnSelectedItemOnErrorsChanged;
    }

    private void OnSelectedItemOnErrorsChanged(object? sender, DataErrorsChangedEventArgs args)
    {
        SaveCommand.NotifyCanExecuteChanged();
    }

    public virtual void LoadData(object obj)
    {
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task OnSave(CancellationToken ct)
    {
        SelectedItem.ValidateAll();
        if (SelectedItem.HasErrors)
        {
            return;
        }

        var result = await ProcessDataAsync(ct);
        if (result.IsSuccess)
        {
            _dialogNavigationService.CloseView();
        }
    }

    [RelayCommand]
    private void OnClose()
    {
        _dialogNavigationService.CloseView();
    }

    protected virtual Task<IProcessResult> ProcessDataAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    private bool CanSave()
    {
        return !SelectedItem.HasErrors;
    }
}