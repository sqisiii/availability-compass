namespace AvailabilityCompass.MauiClient.Shared.Theme;

internal static class SearchTheme
{
    public static readonly Color Accent = Color.FromArgb("#1E88E5");
    public static readonly Color Warning = Color.FromArgb("#FF7043");

    private static readonly Color PrimaryTextLight = Color.FromArgb("#1F2933");
    private static readonly Color PrimaryTextDark = Color.FromArgb("#F3F6FA");
    private static readonly Color MutedTextLight = Color.FromArgb("#5D6673");
    private static readonly Color MutedTextDark = Color.FromArgb("#B8C0CC");
    private static readonly Color AccentTextLight = Color.FromArgb("#1565C0");
    private static readonly Color AccentTextDark = Color.FromArgb("#90CAF9");
    private static readonly Color AvailableTextLight = Color.FromArgb("#2E7D32");
    private static readonly Color AvailableTextDark = Color.FromArgb("#81C784");
    private static readonly Color WarningTextLight = Color.FromArgb("#C2410C");
    private static readonly Color WarningTextDark = Color.FromArgb("#FFB199");
    private static readonly Color PanelLight = Color.FromArgb("#F8F8F8");
    private static readonly Color PanelDark = Color.FromArgb("#24272D");
    private static readonly Color CardLight = Colors.White;
    private static readonly Color CardDark = Color.FromArgb("#2D3037");
    private static readonly Color InputLight = Colors.White;
    private static readonly Color InputDark = Color.FromArgb("#343842");
    private static readonly Color BorderLight = Color.FromArgb("#D7DCE2");
    private static readonly Color BorderDark = Color.FromArgb("#555D6A");
    private static readonly Color ConflictLight = Color.FromArgb("#FFF1EC");
    private static readonly Color ConflictDark = Color.FromArgb("#3A2420");

    public static T PrimaryLabel<T>(T label) where T : Label
    {
        ApplyTextColor(label, PrimaryTextLight, PrimaryTextDark);
        return label;
    }

    public static T MutedLabel<T>(T label) where T : Label
    {
        ApplyTextColor(label, MutedTextLight, MutedTextDark);
        return label;
    }

    public static T AccentLabel<T>(T label) where T : Label
    {
        ApplyTextColor(label, AccentTextLight, AccentTextDark);
        return label;
    }

    public static T AvailableLabel<T>(T label) where T : Label
    {
        ApplyTextColor(label, AvailableTextLight, AvailableTextDark);
        return label;
    }

    public static T WarningLabel<T>(T label) where T : Label
    {
        ApplyTextColor(label, WarningTextLight, WarningTextDark);
        return label;
    }

    public static Entry SearchEntry(Entry entry)
    {
        ApplyTextColor(entry, PrimaryTextLight, PrimaryTextDark);
        ApplyColor(entry, Entry.PlaceholderColorProperty, MutedTextLight, MutedTextDark);
        ApplyBackground(entry, InputLight, InputDark);
        return entry;
    }

    public static Picker SearchPicker(Picker picker)
    {
        ApplyTextColor(picker, PrimaryTextLight, PrimaryTextDark);
        ApplyColor(picker, Picker.TitleColorProperty, MutedTextLight, MutedTextDark);
        ApplyBackground(picker, InputLight, InputDark);
        return picker;
    }

    public static Button PrimaryButton(Button button)
    {
        ApplyColor(button, Button.TextColorProperty, Colors.White, Colors.White);
        ApplyBackground(button, Accent, Accent);
        return button;
    }

    public static T Panel<T>(T view) where T : VisualElement
    {
        ApplyBackground(view, PanelLight, PanelDark);
        return view;
    }

    public static T Card<T>(T border) where T : Border
    {
        ApplyBackground(border, CardLight, CardDark);
        ApplyStroke(border, BorderLight, BorderDark);
        return border;
    }

    public static T InputBorder<T>(T border) where T : Border
    {
        ApplyBackground(border, InputLight, InputDark);
        ApplyStroke(border, BorderLight, BorderDark);
        return border;
    }

    public static T OptionsPanel<T>(T border) where T : Border
    {
        ApplyBackground(border, PanelLight, PanelDark);
        ApplyStroke(border, BorderLight, BorderDark);
        return border;
    }

    public static BoxView Divider(BoxView divider)
    {
        ApplyColor(divider, BoxView.ColorProperty, BorderLight, BorderDark);
        return divider;
    }

    public static View ConflictPanel(View view)
    {
        ApplyBackground(view, ConflictLight, ConflictDark);
        return view;
    }

    public static void ApplyTextColor(Label label, Color light, Color dark) =>
        ApplyColor(label, Label.TextColorProperty, light, dark);

    private static void ApplyTextColor(InputView input, Color light, Color dark) =>
        ApplyColor(input, InputView.TextColorProperty, light, dark);

    private static void ApplyTextColor(Picker picker, Color light, Color dark) =>
        ApplyColor(picker, Picker.TextColorProperty, light, dark);

    private static void ApplyBackground(VisualElement element, Color light, Color dark) =>
        ApplyColor(element, VisualElement.BackgroundColorProperty, light, dark);

    private static void ApplyStroke(Border border, Color light, Color dark)
    {
        border.SetAppThemeColor(Border.StrokeProperty, light, dark);
    }

    private static void ApplyColor(BindableObject target, BindableProperty property, Color light, Color dark)
    {
        target.SetAppThemeColor(property, light, dark);
    }
}
