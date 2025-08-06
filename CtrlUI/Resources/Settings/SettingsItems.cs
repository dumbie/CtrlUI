using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using static ArnoldVinkCode.AVArrayFunctions;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkStyles.AVImage;
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
                //Menu buttons
                listView_SettingsMenu.Items.Add(new DataBindString()
                {
                    Name = "Startup",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/AppRunExe.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                });

                listView_SettingsMenu.Items.Add(new DataBindString()
                {
                    Name = "Display",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Monitor.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                });

                listView_SettingsMenu.Items.Add(new DataBindString()
                {
                    Name = "Launchers",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Launcher.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                });

                listView_SettingsMenu.Items.Add(new DataBindString()
                {
                    Name = "Interface",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Interface.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                });

                listView_SettingsMenu.Items.Add(new DataBindString()
                {
                    Name = "Sound",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/VolumeUp.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                });

                listView_SettingsMenu.Items.Add(new DataBindString()
                {
                    Name = "File Browser",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Folder.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                });

                listView_SettingsMenu.Items.Add(new DataBindString()
                {
                    Name = "Other",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Hamburger.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                });
                listView_SettingsMenu.SelectedIndex = 0;

                //Launcher settings
                var appLauncherArray = EnumToEnumArray<AppLauncher>().Where(x => x != AppLauncher.Unknown);
                foreach (AppLauncher appLauncher in appLauncherArray)
                {
                    try
                    {
                        BitmapImage imageBitmap = await LoadLauncherImage(appLauncher, vImageLoadSizeApplication, 0);
                        string settingName = "ShowLibrary" + appLauncher.ToString();
                        bool settingEnabled = SettingLoad(vConfigurationCtrlUI, settingName, typeof(bool));
                        listView_LauncherSetting.Items.Add(new LauncherSetting()
                        {
                            AppLauncher = appLauncher,
                            ImageBitmap = imageBitmap,
                            Name = settingName,
                            Enabled = settingEnabled
                        });
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