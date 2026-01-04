using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AvailabilityCompass.Core.Features.SearchRecords;
using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;

// ReSharper disable once CheckNamespace
namespace AvailabilityCompass.WpfClient.Pages;

public partial class SearchView : IDisposable
{
    private IDisposable? _subscription;


    public SearchView()
    {
        InitializeComponent();
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }

    private void SearchView_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not SearchViewModel viewModel || _subscription is not null)
        {
            return;
        }

        _subscription = viewModel.ColumnObservable.Subscribe(UpdateDataGridColumns);
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
                Binding = new Binding($"[{column.PropertyName}]"),
                MaxWidth = 250
            });
        }
    }

    private void UrlIcon_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string url && !string.IsNullOrEmpty(url))
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        // Scroll to the bottom
        if (FindName("MainScrollViewer") is ScrollViewer scrollViewer)
        {
            scrollViewer.ScrollToEnd();
        }
    }
}