using System.Reflection;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

public abstract partial class CrudViewModelBase<T> : ObservableValidator, IDialogViewModel where T : ObservableValidator, new()
{
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;

    [ObservableProperty]
    private T _selectedItem = new T();

    protected CrudViewModelBase(INavigationService<IDialogViewModel> dialogNavigationService)
    {
        _dialogNavigationService = dialogNavigationService;
    }

    public bool IsActive { get; set; }
    public bool IsDialogOpen { get; set; }

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

    protected virtual Task<IProcessResult> ProcessDataAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    private bool CanSave()
    {
        return !HasErrors;
    }
}

public static class ObservableValidatorExtensions
{
    public static void ValidateAll(this ObservableValidator validator)
    {
        // Use reflection to call the protected method
        typeof(ObservableValidator)
            .GetMethod("ValidateAllProperties",
                BindingFlags.Instance |
                BindingFlags.NonPublic)
            ?
            .Invoke(validator, null);
    }
}