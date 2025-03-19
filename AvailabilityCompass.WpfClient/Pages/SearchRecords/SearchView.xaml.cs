using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AvailabilityCompass.Core.Features.Search;
using AvailabilityCompass.Core.Features.Search.FilterFormElements;

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

        viewModel.ColumnObservable.Subscribe(columns => UpdateDataGridColumns(viewModel, columns));
    }

    private void UpdateDataGridColumns(SearchViewModel viewModel, List<ResultColumnDefinition> columns)
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