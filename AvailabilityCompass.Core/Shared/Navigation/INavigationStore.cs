namespace AvailabilityCompass.Core.Shared.Navigation;

public interface INavigationStore<T>
{
    T? CurrentViewModel { get; set; }
    event Action? CurrentViewModelChanged;
}