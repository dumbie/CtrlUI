using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.ProcessClasses;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async void Grid_Popup_Welcome_button_PSRemote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);

                vFilePickerFilterIn = new List<string> { "RemotePlay.exe" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "PS Remote Play";
                vFilePickerDescription = "Please select the PS Remote Play executable:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Remote Play", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                await AddAppToList(dataBindApp, true, true);

                //Disable the icon after selection
                ButtonSender.IsEnabled = false;
                ButtonSender.Opacity = 0.40;
            }
            catch { }
        }

        async void Grid_Popup_Welcome_button_Discord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);

                vFilePickerFilterIn = new List<string> { "Update.exe" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Discord";
                vFilePickerDescription = "Please select the Discord update executable:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Discord", NameExe = "Discord.exe", Argument = "--processStart Discord.exe", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile), LaunchKeyboard = true };
                await AddAppToList(dataBindApp, true, true);

                //Disable the icon after selection
                ButtonSender.IsEnabled = false;
                ButtonSender.Opacity = 0.40;
            }
            catch { }
        }

        async void Grid_Popup_Welcome_button_Battle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);

                vFilePickerFilterIn = new List<string> { "battle.net.exe" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Battle.net";
                vFilePickerDescription = "Please select the Battle.net executable:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Battle.net", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                await AddAppToList(dataBindApp, true, true);

                //Disable the icon after selection
                ButtonSender.IsEnabled = false;
                ButtonSender.Opacity = 0.40;
            }
            catch { }
        }

        async void Grid_Popup_Welcome_button_GoG_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);

                vFilePickerFilterIn = new List<string> { "galaxyclient.exe" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "GoG";
                vFilePickerDescription = "Please select the GoG executable:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "GoG", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                await AddAppToList(dataBindApp, true, true);

                //Disable the icon after selection
                ButtonSender.IsEnabled = false;
                ButtonSender.Opacity = 0.40;
            }
            catch { }
        }

        async void Grid_Popup_Welcome_button_Epic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);

                vFilePickerFilterIn = new List<string> { "EpicGamesLauncher.exe" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Epic";
                vFilePickerDescription = "Please select the Epic Games executable:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Epic", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                await AddAppToList(dataBindApp, true, true);

                //Disable the icon after selection
                ButtonSender.IsEnabled = false;
                ButtonSender.Opacity = 0.40;
            }
            catch { }
        }

        async void Grid_Popup_Welcome_button_Ubisoft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);

                vFilePickerFilterIn = new List<string> { "upc.exe" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Ubisoft";
                vFilePickerDescription = "Please select the Ubisoft Connect executable:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Ubisoft", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                await AddAppToList(dataBindApp, true, true);

                //Disable the icon after selection
                ButtonSender.IsEnabled = false;
                ButtonSender.Opacity = 0.40;
            }
            catch { }
        }

        async void Grid_Popup_Welcome_button_EADesktop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);

                vFilePickerFilterIn = new List<string> { "eadesktop.exe", "origin.exe" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "EA";
                vFilePickerDescription = "Please select the EA Desktop executable:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "EA Desktop", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                await AddAppToList(dataBindApp, true, true);

                //Disable the icon after selection
                ButtonSender.IsEnabled = false;
                ButtonSender.Opacity = 0.40;
            }
            catch { }
        }

        async void Grid_Popup_Welcome_button_Steam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);

                vFilePickerFilterIn = new List<string> { "steam.exe" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Steam";
                vFilePickerDescription = "Please select the Steam executable:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Steam", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile), Argument = "-bigpicture", QuickLaunch = true };
                await AddAppToList(dataBindApp, true, true);

                //Disable the icon after selection
                ButtonSender.IsEnabled = false;
                ButtonSender.Opacity = 0.40;
            }
            catch { }
        }

        async void Grid_Popup_Welcome_button_Spotify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);

                vFilePickerFilterIn = new List<string> { "spotify.exe" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Spotify";
                vFilePickerDescription = "Please select the Spotify executable:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Spotify", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                await AddAppToList(dataBindApp, true, true);

                //Disable the icon after selection
                ButtonSender.IsEnabled = false;
                ButtonSender.Opacity = 0.40;
            }
            catch { }
        }

        async void Grid_Popup_Welcome_button_Kodi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);

                vFilePickerFilterIn = new List<string> { "kodi.exe" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Kodi";
                vFilePickerDescription = "Please select the Kodi executable:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Kodi", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                await AddAppToList(dataBindApp, true, true);

                //Disable the icon after selection
                ButtonSender.IsEnabled = false;
                ButtonSender.Opacity = 0.40;
            }
            catch { }
        }

        async void Grid_Popup_Welcome_button_Edge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);

                vFilePickerFilterIn = new List<string> { "msedge.exe" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Microsoft Edge";
                vFilePickerDescription = "Please select the Microsoft Edge executable:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Edge", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile), LaunchKeyboard = true };
                await AddAppToList(dataBindApp, true, true);

                //Disable the icon after selection
                ButtonSender.IsEnabled = false;
                ButtonSender.Opacity = 0.40;
            }
            catch { }
        }
    }
}