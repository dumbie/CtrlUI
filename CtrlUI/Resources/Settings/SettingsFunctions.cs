using System.Linq;
using System.Threading.Tasks;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show and load Settings window information
        async Task ShowLoadSettingsPopup()
        {
            try
            {
                //Set and load the quick launch application name
                try
                {
                    DataBindApp QuickLaunchApp = CombineAppLists(true, true, true, false, false, false, false).FirstOrDefault(x => x.QuickLaunch);
                    btn_Settings_AppQuickLaunch_TextBlock.Text = "Change quick launch app: " + QuickLaunchApp.Name;
                }
                catch
                {
                    btn_Settings_AppQuickLaunch_TextBlock.Text = "Change the quick launch app";
                }

                //Show the settings popup
                await Popup_Show(grid_Popup_Settings, null);

                //Focus on settings tab
                await Listbox_Settings_SingleTap();
            }
            catch { }
        }

        //Change the settings tab
        async Task SettingsChangeTab(bool changeLeft)
        {
            try
            {
                if (changeLeft)
                {
                    int selectedIndex = listView_SettingsMenu.SelectedIndex;
                    if (selectedIndex > 0)
                    {
                        listView_SettingsMenu.SelectedIndex = listView_SettingsMenu.SelectedIndex - 1;
                        await Listbox_Settings_SingleTap();
                    }
                }
                else
                {
                    listView_SettingsMenu.SelectedIndex = listView_SettingsMenu.SelectedIndex + 1;
                    await Listbox_Settings_SingleTap();
                }
            }
            catch { }
        }
    }
}