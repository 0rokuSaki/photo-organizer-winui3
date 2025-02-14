using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PhotoOrganizer.ViewModels;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PhotoOrganizer;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        Title = "Photo Organizer";
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(TitleBar);

        ViewModel = Ioc.Default.GetService<MainWindowViewModel>();
    }

    public MainWindowViewModel? ViewModel { get; }

    public StorageFolder? SelectedInputFolder { get; set; }

    public StorageFolder? SelectedOutputFolder { get; set; }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        ContentDialogResult result = await StartSettingsDialog.ShowAsync();
        if (result is ContentDialogResult.Primary && ViewModel is not null)
        {
            ViewModel.InputFolder = SelectedInputFolder;
            ViewModel.OutputFolder = SelectedOutputFolder;
        }
    }

    private async Task<StorageFolder?> SelectFolderAsync()
    {
        FolderPicker folderPicker = new();
        folderPicker.FileTypeFilter.Add("*");
        IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
        return await folderPicker.PickSingleFolderAsync();
    }

    private async void SelectedInputFolderButton_Click(object sender, RoutedEventArgs e)
    {
        StorageFolder? folder = await SelectFolderAsync();
        if (folder is not null && ViewModel is not null)
        {
            SelectedInputFolder = folder;
            SelectedInputFolderTextBox.Text = folder.Path;
        }
    }

    private async void SelectedOutputFolderButton_Click(object sender, RoutedEventArgs e)
    {
        StorageFolder? folder = await SelectFolderAsync();
        if (folder is not null && ViewModel is not null)
        {
            SelectedOutputFolder = folder;
            SelectedOutputFolderTextBox.Text = folder.Path;
        }
    }

    private void FolderStructureCheckBox_Click(object sender, RoutedEventArgs e) =>
        UpdateOutputFolderExample();

    private void UpdateOutputFolderExample()
    {
        string example = @"[Output]";

        if (SelectedOutputFolder?.Path.Length > 0)
        {
            example = SelectedOutputFolder.Path;
        }

        string dateFormat = CreateDateFolderFormat();
        example += DateTime.Now.ToString(dateFormat, CultureInfo.InvariantCulture);
        example += @"\[FileName]";

        ExampleTextBlock.Text = example;
    }

    private string CreateDateFolderFormat()
    {
        string format = string.Empty;

        if (CreateYearFolderCheckBox.IsChecked is true)
        {
            format += @"\\yyyy";
        }
        if (CreateMonthFolderCheckBox.IsChecked is true)
        {
            format += @"\\MM";
        }
        if (CreateDayFolderCheckBox.IsChecked is true)
        {
            format += @"\\dd";
        }
        if (CreateDateFolderCheckBox.IsChecked is true)
        {
            format += @"\\yyyy-MM-dd";
        }

        return format;
    }
}
