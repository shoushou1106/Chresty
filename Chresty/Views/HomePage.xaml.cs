using Chresty.ViewModels;
using Frosty.Core;
using Frosty.Core.Mod;
using FrostySdk;
using FrostyCore;
using FrostySdk.BaseProfile;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using System.Collections.Generic;
using FrostySdk.Interfaces;
using Microsoft.Extensions.Logging;
using Chresty.Services;
using System.Reflection;

namespace Chresty.Views;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel
    {
        get;
    }

    public HomePage()
    {
        ViewModel = App.GetService<HomeViewModel>();
        InitializeComponent();
        InitializeShitMountain();
    }

    public static FrostySdk.Interfaces.ILogger Logger { get => Frosty.Core.App.Logger; set => Frosty.Core.App.Logger = value; }

    public static PluginManager PluginManager { get => Frosty.Core.App.PluginManager; set => Frosty.Core.App.PluginManager = value; }

    public void InitializeShitMountain()
    {
        Logger = new FakeFrostyLogger();
        PluginManager = new PluginManager(Logger, PluginManagerType.ModManager);
        ProfilesLibrary.Initialize(PluginManager.Profiles);
    }

    // Retrieve the window handle (HWND) of the current WinUI 3 window.
    private nint hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

    private IFrostyMod Mod = null;

    private async void SelectMod()
    {
        // Create a file picker
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

        // Initialize the file picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.FileTypeFilter.Add(".fbmod");

        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();

        if (file != null)
        {
            using FileStream stream = new FileStream(file.Path, FileMode.Open, FileAccess.Read);

            int retCode = 0;

            int format = (retCode & 0x8000) != 0 ? 1 : 0;

            FrostyMod mod = null;
            mod = new FrostyMod(file.Path);

            IFrostyMod Mod = mod;

            ExportProject();
        }
        else
        {
            ShowDialog("Error", "choose is null");
        }

    }

    private async void ExportProject()
    {
        const uint FormatVersion = 15;
        const ulong Magic = 0x00005954534F5246;

        // Create a file picker
        FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();

        // Dropdown of file types the user can save the file as
        savePicker.FileTypeChoices.Add("Frosty Project", new List<string>() { ".fbproject" });

        // Open the picker for the user to pick a file
        StorageFile file = await savePicker.PickSaveFileAsync();

        if (file != null)
        {
            // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
            CachedFileManager.DeferUpdates(file);

            // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
            // Completing updates may require Windows to ask for user input.
            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
            if (status == FileUpdateStatus.Complete)
            {
                ShowDialog("info", "was saved");
            }
            else if (status == FileUpdateStatus.CompleteAndRenamed)
            {
                ShowDialog("info", "was renamed and saved");
            }
            else
            {
                ShowDialog("error", "couldn't be saved");
            }
        }

    }

    private async void SelectProfile()
    {
        // Create a file picker
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

        // Initialize the file picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.FileTypeFilter.Add(".exe");

        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();

        FileInfo fi = new FileInfo(file.Path);

        if (file != null)
        {
            // try to load game profile 
            if (!ProfilesLibrary.HasProfile(fi.Name.Remove(fi.Name.Length - 4)))
            {
                ShowDialog("profile not exist", fi.Name.Remove(file.Name.Length - 4));
                return;
            }
            ChooseProfile.IsEnabled = false;
            ImportMod.IsEnabled = true;
            if (!ProfilesLibrary.Initialize(file.Name.Remove(file.Name.Length - 4)))
            {
                ShowDialog("error", "unknown");
                return;
            }
            if (!ProfilesLibrary.EnableExecution)
            {
                ShowDialog("Warning", "Frosty didnt support this game but we will try it, restart app to select again if you want");
            }
        }
        else
        {
            ShowDialog("Error", "file is null");
        }
    }

    private async void ShowDialog(string title, string content)
    {
        ContentDialog dialog = new ContentDialog();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = XamlRoot;
        dialog.Title = title;
        dialog.Content = content;
        dialog.PrimaryButtonText = "OK";
        dialog.DefaultButton = ContentDialogButton.Primary;

        var result = await dialog.ShowAsync();
    }

    private void ImportMod_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ProgressRing.IsActive = true;
        SelectMod();
        ProgressRing.IsActive = false;
    }

    private void ChooseProfile_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ProgressRing.IsActive = true;
        SelectProfile();
        ProgressRing.IsActive = false;
    }
}
