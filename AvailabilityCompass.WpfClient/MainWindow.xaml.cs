using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace AvailabilityCompass.WpfClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const int DefaultLowOrderWordMask = 0xFFFF;
    private const int DefaultHighOrderWordMask = 16;
    private const int DefaultWmNonClientHitTest = 0x84;
    private const int DefaultHitTestTopLeft = 13;
    private const int DefaultHitTestTopRight = 14;
    private const int DefaultHitTestBottomLeft = 16;
    private const int DefaultHitTestBottomRight = 17;
    private const int DefaultHitTestLeft = 10;
    private const int DefaultHitTestRight = 11;
    private const int DefaultHitTestTop = 12;
    private const int DefaultHitTestBottom = 15;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
    }


    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

    private void ControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            if (BtnMaximize.Command?.CanExecute(null) == true)
            {
                BtnMaximize.Command.Execute(null);
            }
        }
        else
        {
            WindowInteropHelper helper = new(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }
    }

    private void ControlBar_MouseEnter(object sender, MouseEventArgs e)
    {
        MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            Mouse.OverrideCursor = Cursors.Wait;
        }

        System.Windows.Application.Current.Shutdown();
    }

    private void BtnMinimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        // Handle window messages for resizing and moving
        return msg switch
        {
            DefaultWmNonClientHitTest => WmNonClientHitTest(lParam, out handled),
            _ => IntPtr.Zero
        };
    }

    private IntPtr WmNonClientHitTest(IntPtr lParam, out bool handled)
    {
        // Determine hit test area for resizing or moving
        Point ptScreen = new(lParam.ToInt32() & DefaultLowOrderWordMask, lParam.ToInt32() >> DefaultHighOrderWordMask);
        Point ptClient = PointFromScreen(ptScreen);

        const int resizeBorderWidth = 8;

        if (ptClient.X <= resizeBorderWidth && ptClient.Y <= resizeBorderWidth)
        {
            handled = true;
            return (IntPtr)DefaultHitTestTopLeft;
        }

        if (ptClient.X >= ActualWidth - resizeBorderWidth && ptClient.Y <= resizeBorderWidth)
        {
            handled = true;
            return (IntPtr)DefaultHitTestTopRight;
        }

        if (ptClient.X <= resizeBorderWidth && ptClient.Y >= ActualHeight - resizeBorderWidth)
        {
            handled = true;
            return (IntPtr)DefaultHitTestBottomLeft;
        }

        if (ptClient.X >= ActualWidth - resizeBorderWidth && ptClient.Y >= ActualHeight - resizeBorderWidth)
        {
            handled = true;
            return (IntPtr)DefaultHitTestBottomRight;
        }

        if (ptClient.X <= resizeBorderWidth)
        {
            handled = true;
            return (IntPtr)DefaultHitTestLeft;
        }

        if (ptClient.X >= ActualWidth - resizeBorderWidth &&
            ptClient.X <= SystemParameters.VirtualScreenWidth - SystemParameters.BorderWidth)
        {
            handled = true;
            return (IntPtr)DefaultHitTestRight;
        }

        if (ptClient.Y <= resizeBorderWidth)
        {
            handled = true;
            return (IntPtr)DefaultHitTestTop;
        }

        if (ptClient.Y >= ActualHeight - resizeBorderWidth)
        {
            handled = true;
            return (IntPtr)DefaultHitTestBottom;
        }

        handled = false;
        return IntPtr.Zero;
    }

    private void BtnMaximize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
    }
}