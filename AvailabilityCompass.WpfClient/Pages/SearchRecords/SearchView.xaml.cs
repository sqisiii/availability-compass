using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AvailabilityCompass.Core.Features.Search;

// ReSharper disable once CheckNamespace
namespace AvailabilityCompass.WpfClient.Pages;

public partial class SearchView : UserControl
{
    public SearchView()
    {
        InitializeComponent();
    }

    private void SearchView_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not SearchViewModel viewModel)
        {
            return;
        }

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.Columns))
            {
                UpdateDataGridColumns(viewModel);
            }
        };

        UpdateDataGridColumns(viewModel);
    }

    private void UpdateDataGridColumns(SearchViewModel viewModel)
    {
        ResultsDataGridView.Columns.Clear();
        foreach (var column in viewModel.Columns)
        {
            ResultsDataGridView.Columns.Add(new DataGridTextColumn
            {
                Header = column.Header,
                Binding = new Binding($"[{column.PropertyName}]")
            });
        }
    }
}