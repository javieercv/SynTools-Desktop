using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using SynTools.Probe.Core;

namespace SynTools.Probe.App;

public sealed class MainWindow : Window
{
    private readonly Border _content = new() { Padding = new Thickness(24) };
    private TextBlock _status = new() { Text = "Preparado", Margin = new Thickness(0, 12, 0, 0) };
    private readonly OperationCoordinator _coordinator = new();
    private OperationLease? _operation;

    public MainWindow()
    {
        Title = "SynTools · Prueba técnica Avalonia";
        Width = 1100; Height = 720; MinWidth = 760; MinHeight = 520;
        var initialSection = Environment.GetCommandLineArgs().Contains("--render", StringComparer.Ordinal) ? 1 : 0;
        var navigation = new ListBox { Width = 220, Margin = new Thickness(12), ItemsSource = new[] { "Inicio", "Herramienta de prueba", "Historial", "Ajustes" }, SelectedIndex = initialSection };
        navigation.SelectionChanged += (_, _) => ShowSection(navigation.SelectedItem?.ToString() ?? "Inicio");
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("220,*") };
        grid.Children.Add(navigation); Grid.SetColumn(_content, 1); grid.Children.Add(_content);
        Content = grid;
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
        KeyDown += OnKeyDown;
        ShowSection(initialSection == 1 ? "Herramienta de prueba" : "Inicio");
    }

    private void ShowSection(string section)
    {
        _content.Child = section switch
        {
            "Herramienta de prueba" => ToolView(),
            "Historial" => Page("Historial", "Estado vacío · no se almacenan rutas privadas."),
            "Ajustes" => SettingsView(),
            _ => Page("Inicio", "Shell compartida, redimensionable y con UI en español.")
        };
    }

    private Control ToolView()
    {
        _status = new TextBlock { Text = "Preparado", Margin = new Thickness(0, 12, 0, 0) };
        var waveform = new WaveformControl();
        var slider = new Slider { Minimum = 1, Maximum = 12, Value = 1 };
        slider.PropertyChanged += (_, args) => { if (args.Property == Slider.ValueProperty) { waveform.Zoom = slider.Value; waveform.InvalidateVisual(); } };
        var scroll = new Slider { Minimum = 0, Maximum = 1, Value = 0 };
        scroll.PropertyChanged += (_, args) => { if (args.Property == Slider.ValueProperty) { waveform.Offset = scroll.Value; waveform.InvalidateVisual(); } };
        var panel = new StackPanel { Spacing = 10 };
        panel.Children.Add(new TextBlock { Text = "Herramienta de prueba", FontSize = 28 });
        panel.Children.Add(new TextBlock { Text = "Arrastra archivos aquí o usa los selectores. ⌘/Ctrl+O abre un archivo; ⌘/Ctrl+F alterna pantalla completa." });
        panel.Children.Add(Row(Button("Seleccionar archivo…", PickFile), Button("Seleccionar varios…", PickFiles), Button("Seleccionar carpeta…", PickFolder)));
        panel.Children.Add(Row(Button("Iniciar operación", StartOperation), Button("Cancelar", CancelOperation), Button("Revelar temporales", RevealTemporary)));
        panel.Children.Add(new TextBlock { Text = "Zoom" }); panel.Children.Add(slider); panel.Children.Add(new TextBlock { Text = "Desplazamiento" }); panel.Children.Add(scroll); panel.Children.Add(waveform); panel.Children.Add(_status);
        return new ScrollViewer { Content = panel };
    }

    private Control SettingsView()
    {
        var themes = new ComboBox { ItemsSource = new[] { "Sistema", "Claro", "Oscuro" }, SelectedIndex = 0, Width = 180, HorizontalAlignment = HorizontalAlignment.Left };
        themes.SelectionChanged += (_, _) => Application.Current!.RequestedThemeVariant = themes.SelectedIndex switch { 1 => ThemeVariant.Light, 2 => ThemeVariant.Dark, _ => ThemeVariant.Default };
        return new StackPanel { Spacing = 12, Children = { new TextBlock { Text = "Ajustes", FontSize = 28 }, new TextBlock { Text = "Tema" }, themes } };
    }

    private static StackPanel Page(string title, string description) => new() { Spacing = 12, Children = { new TextBlock { Text = title, FontSize = 28 }, new TextBlock { Text = description } } };
    private static Button Button(string text, EventHandler<RoutedEventArgs> action) { var button = new Button { Content = text }; button.Click += action; return button; }
    private static StackPanel Row(params Control[] controls) { var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 }; foreach (var control in controls) panel.Children.Add(control); return panel; }

    private async void PickFile(object? sender, RoutedEventArgs e) { var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { Title = "Seleccionar archivo", AllowMultiple = false }); _status.Text = files.Count == 0 ? "Selección cancelada" : $"Archivo: {files[0].Name}"; }
    private async void PickFiles(object? sender, RoutedEventArgs e) { var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { Title = "Seleccionar varios", AllowMultiple = true }); _status.Text = $"Archivos seleccionados: {files.Count}"; }
    private async void PickFolder(object? sender, RoutedEventArgs e) { var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = "Seleccionar carpeta", AllowMultiple = false }); _status.Text = folders.Count == 0 ? "Selección cancelada" : $"Carpeta: {folders[0].Name}"; }
    private void StartOperation(object? sender, RoutedEventArgs e) { try { _operation = _coordinator.Begin("Prueba", indeterminate: true); _status.Text = "Operación indeterminada activa"; } catch (InvalidOperationException error) { _status.Text = error.Message; } }
    private void CancelOperation(object? sender, RoutedEventArgs e) { if (_operation is null) return; _operation.Cancel(); _operation.AcknowledgeCancellation(); _operation.Dispose(); _operation = null; _status.Text = "Operación cancelada y reserva liberada"; }
    private void RevealTemporary(object? sender, RoutedEventArgs e) { var uri = new Uri(Path.GetTempPath()); TopLevel.GetTopLevel(this)?.Launcher.LaunchUriAsync(uri); }
    private void OnDragOver(object? sender, DragEventArgs e) { e.DragEffects = e.DataTransfer.Contains(DataFormat.File) ? DragDropEffects.Copy : DragDropEffects.None; }
    private void OnDrop(object? sender, DragEventArgs e) { var items = e.DataTransfer.TryGetFiles(); _status.Text = items is null ? "Entrada no compatible" : $"Elementos arrastrados: {items.Count()}"; }
    private void OnKeyDown(object? sender, KeyEventArgs e) { var command = e.KeyModifiers.HasFlag(KeyModifiers.Meta) || e.KeyModifiers.HasFlag(KeyModifiers.Control); if (command && e.Key == Key.O) { PickFile(this, new RoutedEventArgs()); e.Handled = true; } if (command && e.Key == Key.F) { WindowState = WindowState == WindowState.FullScreen ? WindowState.Normal : WindowState.FullScreen; e.Handled = true; } }
}
