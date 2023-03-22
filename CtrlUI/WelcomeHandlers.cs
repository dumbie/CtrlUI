using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async void Grid_Popup_Welcome_button_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Close top popup
                await Popup_Close_Top(false);

                //Enable main menu
                MainMenuButtonsEnable(true);
            }
            catch { }
        }

        async void Grid_Popup_Welcome_button_PSRemote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "RemotePlay.exe" };
                vFilePickerSettings.Title = "PS Remote Play";
                vFilePickerSettings.Description = "Please select the PS Remote Play executable:";
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

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "Discord.exe", "Update.exe" };
                vFilePickerSettings.Title = "Discord";
                vFilePickerSettings.Description = "Please select the Discord executable:";
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

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "battle.net.exe" };
                vFilePickerSettings.Title = "Battle.net";
                vFilePickerSettings.Description = "Please select the Battle.net executable:";
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

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "galaxyclient.exe" };
                vFilePickerSettings.Title = "GoG";
                vFilePickerSettings.Description = "Please select the GoG executable:";
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

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "EpicGamesLauncher.exe" };
                vFilePickerSettings.Title = "Epic";
                vFilePickerSettings.Description = "Please select the Epic Games executable:";
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

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "upc.exe" };
                vFilePickerSettings.Title = "Ubisoft";
                vFilePickerSettings.Description = "Please select the Ubisoft Connect executable:";
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

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "eadesktop.exe", "origin.exe" };
                vFilePickerSettings.Title = "EA";
                vFilePickerSettings.Description = "Please select the EA Desktop executable:";
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

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "steam.exe" };
                vFilePickerSettings.Title = "Steam";
                vFilePickerSettings.Description = "Please select the Steam executable:";
                await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Steam", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile), Argument = "-bigpicture" };
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

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "spotify.exe" };
                vFilePickerSettings.Title = "Spotify";
                vFilePickerSettings.Description = "Please select the Spotify executable:";
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

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "kodi.exe" };
                vFilePickerSettings.Title = "Kodi";
                vFilePickerSettings.Description = "Please select the Kodi executable:";
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

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "msedge.exe" };
                vFilePickerSettings.Title = "Microsoft Edge";
                vFilePickerSettings.Description = "Please select the Microsoft Edge executable:";
                await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Edge", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile), LaunchKeyboard = true, QuickLaunch = true };
                await AddAppToList(dataBindApp, true, true);

                //Disable the icon after selection
                ButtonSender.IsEnabled = false;
                ButtonSender.Opacity = 0.40;
            }
            catch { }
        }
    }
}