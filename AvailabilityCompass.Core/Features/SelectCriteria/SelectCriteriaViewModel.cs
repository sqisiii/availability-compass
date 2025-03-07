using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.SelectCriteria;

[ObservableRecipient]
public partial class SelectCriteriaViewModel : ObservableValidator, IPageViewModel
{
    public string Icon => "SearchWeb";
    public string Name => "Search";
}