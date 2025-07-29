using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using static ArnoldVinkCode.AVArrayFunctions;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Items - Application Settings
        async Task Settings_Items()
        {
            try
            {
                //Launcher settings
                var appLauncherArray = EnumToEnumArray<AppLauncher>().Where(x => x != AppLauncher.Unknown);
                foreach (AppLauncher appLauncher in appLauncherArray)
                {
                    try
                    {
                        BitmapImage imageBitmap = await LoadLauncherImage(appLauncher, vImageLoadSizeApplication, 0);
                        string settingName = "ShowLibrary" + appLauncher.ToString();
                        bool settingEnabled = SettingLoad(vConfigurationCtrlUI, settingName, typeof(bool));
                        listView_LauncherSetting.Items.Add(new LauncherSetting() { AppLauncher = appLauncher, ImageBitmap = imageBitmap, Name = settingName, Enabled = settingEnabled });
                    }
                    catch { }
                }
                listView_LauncherSetting.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to set setting items: " + ex.Message);
            }
        }
    }
}