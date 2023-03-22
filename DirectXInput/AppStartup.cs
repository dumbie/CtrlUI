using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static ArnoldVinkCode.AVJsonFunctions;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.Styles.MainColors;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    public partial class WindowMain : Window
    {
        //Run application startup code
        public async Task Startup()
        {
            try
            {
                //Initialize Settings
                Settings_Check();
                await Settings_Load();
                Settings_Save();

                //Change application accent color
                string colorLightHex = SettingLoad(vConfigurationCtrlUI, "ColorAccentLight", typeof(string));
                ChangeApplicationAccentColor(colorLightHex);
                vApplicationAccentLightBrush = (SolidColorBrush)Application.Current.Resources["ApplicationAccentLightBrush"];

                //Create the tray menu
                Application_CreateTrayMenu();

                //Check if application has launched as admin
                if (vAdministratorPermission)
                {
                    this.Title += " (Admin)";
                }

                //Check settings if window needs to be shown
                if (SettingLoad(vConfigurationDirectXInput, "AppFirstLaunch", typeof(bool)))
                {
                    Debug.WriteLine("First launch showing the window.");
                    Application_ShowHideWindow();
                }

                //Check if drivers are installed
                if (!CheckInstalledDrivers())
                {
                    if (!ShowInTaskbar) { Application_ShowHideWindow(); }
                    await Message_InstallDrivers();
                    return;
                }

                //Check installed driver versions
                if (!CheckDriversVersion())
                {
                    if (!ShowInTaskbar) { Application_ShowHideWindow(); }
                    await Message_UpdateDrivers();
                    return;
                }

                //Open the virtual bus driver
                if (!await OpenVirtualBusDriver())
                {
                    if (!ShowInTaskbar) { Application_ShowHideWindow(); }
                    await Message_InstallDrivers();
                    return;
                }

                //Open the hid hide device
                if (!OpenHidHideDevice())
                {
                    if (!ShowInTaskbar) { Application_ShowHideWindow(); }
                    await Message_InstallDrivers();
                    return;
                }

                //Open the FakerInput device
                if (!OpenFakerInputDevice())
                {
                    if (!ShowInTaskbar) { Application_ShowHideWindow(); }
                    await Message_InstallDrivers();
                    return;
                }

                //Check settings if Screen Capture Tool launches on start
                if (SettingLoad(vConfigurationDirectXInput, "ShortcutScreenshotController", typeof(bool)))
                {
                    ProcessLaunch.LaunchScreenCaptureTool(true, true);
                }

                //Load the help text
                LoadHelp();

                //Register Interface Handlers
                RegisterInterfaceHandlers();

                //Load combobox values
                ComboBox_MapKeypad_Load();

                //Load application close tools
                JsonLoadFile(ref vDirectCloseTools, @"Profiles\Default\DirectCloseTools.json");

                //Close running controller tools
                CloseControllerTools();

                //Load keyboard emoji and text list
                JsonLoadFile(ref vDirectKeyboardEmojiListActivity, @"Profiles\Default\DirectKeyboardEmojiListActivity.json");
                JsonLoadFile(ref vDirectKeyboardEmojiListNature, @"Profiles\Default\DirectKeyboardEmojiListNature.json");
                JsonLoadFile(ref vDirectKeyboardEmojiListFood, @"Profiles\Default\DirectKeyboardEmojiListFood.json");
                JsonLoadFile(ref vDirectKeyboardEmojiListOther, @"Profiles\Default\DirectKeyboardEmojiListOther.json");
                JsonLoadFile(ref vDirectKeyboardEmojiListPeople, @"Profiles\Default\DirectKeyboardEmojiListPeople.json");
                JsonLoadFile(ref vDirectKeyboardEmojiListSmiley, @"Profiles\Default\DirectKeyboardEmojiListSmiley.json");
                JsonLoadFile(ref vDirectKeyboardEmojiListSymbol, @"Profiles\Default\DirectKeyboardEmojiListSymbol.json");
                JsonLoadFile(ref vDirectKeyboardEmojiListTravel, @"Profiles\Default\DirectKeyboardEmojiListTravel.json");
                JsonLoadFile(ref vDirectKeyboardTextList, @"Profiles\User\DirectKeyboardTextList.json");

                //Load keypad mapping
                JsonLoadMulti(vDirectKeypadMapping, @"Profiles\User\DirectKeypadMapping", true);
                UpdateKeypadInterface();

                //Load controllers supported
                JsonLoadMulti(vDirectControllersSupported, @"Profiles\Default\DirectControllersSupported", true);

                //Load controllers profile
                JsonLoadMulti(vDirectControllersProfile, @"Profiles\User\DirectControllersProfile", true);

                //Load controllers ignored
                JsonLoadFile(ref vDirectControllersIgnored, @"Profiles\User\DirectControllersIgnored.json");

                //Bind all the lists to ListBox
                ListBoxBindLists();

                //Reset HidHide to defaults
                vHidHideDevice.ListDeviceReset();
                vHidHideDevice.ListApplicationReset();

                //Allow DirectXInput in HidHide
                string appFilePath = Assembly.GetEntryAssembly().Location;
                vHidHideDevice.ListApplicationAdd(appFilePath);

                //Enable HidHide device
                vHidHideDevice.DeviceHideToggle(true);

                //Start the background tasks
                TasksBackgroundStart();

                //Register keyboard hotkeys
                AVInputOutputHotKey.Start();
                AVInputOutputHotKey.EventHotKeyPressed += EventHotKeyPressed;

                //Set application first launch to false
                SettingSave(vConfigurationDirectXInput, "AppFirstLaunch", "False");

                //Enable the socket server
                await EnableSocketServer();
            }
            catch { }
        }
    }
}