using ArnoldVinkCode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle main menu mouse/touch tapped
        async void ListBox_Menu_MousePressUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    vSingleTappedEvent = true;
                    await Task.Delay(500);
                    if (vSingleTappedEvent) { await listbox_Menu_SingleTap(); }
                }
            }
            catch { }
        }

        //Handle main menu single tap
        async Task listbox_Menu_SingleTap()
        {
            try
            {
                if (listbox_MainMenu.SelectedIndex >= 0)
                {
                    StackPanel SelStackPanel = (StackPanel)listbox_MainMenu.SelectedItem;
                    if (SelStackPanel.Name == "menuButtonMenu") { await Popup_ShowHide_MainMenu(false); }
                    else if (SelStackPanel.Name == "menuButtonFullScreen") { await AppSwitchScreenMode(false, false); }
                    else if (SelStackPanel.Name == "menuButtonMoveMonitor") { await AppMoveMonitor(); }
                    else if (SelStackPanel.Name == "menuButtonSwitchMonitor") { await SwitchDisplayMonitor(); }
                    else if (SelStackPanel.Name == "menuButtonWindowsStart") { ShowWindowStartMenu(); }
                    else if (SelStackPanel.Name == "menuButtonSearch") { await Popup_ShowHide_Search(true); }
                    else if (SelStackPanel.Name == "menuButtonSorting") { SortAppLists(false, false); }
                    else if (SelStackPanel.Name == "menuButtonMediaControl") { await Popup_Show(grid_Popup_Media, grid_Popup_Media_PlayPause, true); }
                    else if (SelStackPanel.Name == "menuButtonAudioDevice") { await SwitchAudioDevice(); }
                    else if (SelStackPanel.Name == "menuButtonRunExe") { await RunExecutableFile(); }
                    else if (SelStackPanel.Name == "menuButtonAppRun") { await RunUwpApplication(); }
                    else if (SelStackPanel.Name == "menuButtonAppAdd") { await Popup_Show_AppAdd(); }
                    else if (SelStackPanel.Name == "menuButtonFps") { CloseShowFpsOverlayer(); }
                    else if (SelStackPanel.Name == "menuButtonSettings") { await ShowLoadSettingsPopup(); }
                    else if (SelStackPanel.Name == "menuButtonHelp") { await Popup_Show(grid_Popup_Help, btn_Help_Focus, true); }
                    else if (SelStackPanel.Name == "menuButtonCloseLaunchers") { await CloseLaunchers(false); }
                    else if (SelStackPanel.Name == "menuButtonDisconnect") { await CloseStreamers(); }
                    else if (SelStackPanel.Name == "menuButtonShutdown") { await Popup_Show_MessageBox_Shutdown(); }
                    else if (SelStackPanel.Name == "menuButtonEmptyRecycleBin") { RecycleBin_Empty(); }
                }
            }
            catch { }
        }
    }
}