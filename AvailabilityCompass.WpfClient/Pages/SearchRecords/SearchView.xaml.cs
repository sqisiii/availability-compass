using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
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

        viewModel.ColumnObservable.Subscribe(UpdateDataGridColumns);
    }

    private void UpdateDataGridColumns(List<ResultColumnDefinition> columns)
    {
        ResultsDataGridView.Columns.Clear();
        foreach (var column in columns)
        {
            if (column.PropertyName == "Url")
            {
                ResultsDataGridView.Columns.Add(new DataGridTemplateColumn
                {
                    Header = column.Header,
                    CellTemplate = (DataTemplate)FindResource("UrlCellTemplate")
                });
                continue;
            }

            ResultsDataGridView.Columns.Add(new DataGridTextColumn
            {
                Header = column.Header,
                Binding = new Binding($"[{column.PropertyName}]")
            });
        }
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }
}