using ArnoldVinkCode.Styles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVJsonFunctions;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Classes;

namespace FpsOverlayer
{
    public partial class WindowSettings : Window
    {
        //Window Initialize
        public WindowSettings() { InitializeComponent(); }

        //Window Initialized
        protected override async void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Load available fonts
                LoadAvailableFonts();

                //Load and save settings
                await Settings_Load();
                Settings_Save();

                //Load and save shortcuts
                Shortcuts_Load();
                Shortcuts_Save();

                //Bind the lists to the listbox elements
                ListBoxBindLists();

                //Register Interface Handlers
                RegisterInterfaceHandlers();
            }
            catch { }
        }

        //Load available fonts
        void LoadAvailableFonts()
        {
            try
            {
                //Clear all the fonts
                combobox_InterfaceFontStyleName.Items.Clear();

                //Add default fonts
                combobox_InterfaceFontStyleName.Items.Add("Segoe UI");
                combobox_InterfaceFontStyleName.Items.Add("Verdana");
                combobox_InterfaceFontStyleName.Items.Add("Consolas");
                combobox_InterfaceFontStyleName.Items.Add("Arial");

                //Add custom fonts
                DirectoryInfo directoryInfoUser = new DirectoryInfo("Assets/User/Fonts");
                FileInfo[] fontFilesUser = directoryInfoUser.GetFiles("*.ttf", SearchOption.TopDirectoryOnly);
                DirectoryInfo directoryInfoDefault = new DirectoryInfo("Assets/Default/Fonts");
                FileInfo[] fontFilesDefault = directoryInfoDefault.GetFiles("*.ttf", SearchOption.TopDirectoryOnly);
                IEnumerable<FileInfo> fontFiles = fontFilesUser.Concat(fontFilesDefault);

                foreach (FileInfo fontFile in fontFiles)
                {
                    combobox_InterfaceFontStyleName.Items.Add(Path.GetFileNameWithoutExtension(fontFile.Name));
                }
            }
            catch { }
        }

        //Register Interface Handlers
        void RegisterInterfaceHandlers()
        {
            try
            {
                //Main menu functions
                lb_Menu.PreviewKeyUp += lb_Menu_KeyPressUp;
                lb_Menu.PreviewMouseUp += lb_Menu_MousePressUp;

                button_AddApp.Click += Button_AddApp_Click;
                textbox_AddApp.PreviewKeyDown += Textbox_AddApp_PreviewKeyDown;

                button_GpuUp.Click += Button_MoveUp_Click;
                button_GpuDown.Click += Button_MoveDown_Click;

                button_CpuUp.Click += Button_MoveUp_Click;
                button_CpuDown.Click += Button_MoveDown_Click;

                button_MemUp.Click += Button_MoveUp_Click;
                button_MemDown.Click += Button_MoveDown_Click;

                button_NetUp.Click += Button_MoveUp_Click;
                button_NetDown.Click += Button_MoveDown_Click;

                button_FpsUp.Click += Button_MoveUp_Click;
                button_FpsDown.Click += Button_MoveDown_Click;

                button_FrametimeUp.Click += Button_MoveUp_Click;
                button_FrametimeDown.Click += Button_MoveDown_Click;

                button_AppUp.Click += Button_MoveUp_Click;
                button_AppDown.Click += Button_MoveDown_Click;

                button_BatUp.Click += Button_MoveUp_Click;
                button_BatDown.Click += Button_MoveDown_Click;

                button_TimeUp.Click += Button_MoveUp_Click;
                button_TimeDown.Click += Button_MoveDown_Click;

                button_CustomTextUp.Click += Button_MoveUp_Click;
                button_CustomTextDown.Click += Button_MoveDown_Click;

                button_MonUp.Click += Button_MoveUp_Click;
                button_MonDown.Click += Button_MoveDown_Click;

                button_FanUp.Click += Button_MoveUp_Click;
                button_FanDown.Click += Button_MoveDown_Click;

                button_BrowserShowHide.Click += Button_BrowserShowHide_Click;
                button_CrosshairShowHide.Click += Button_CrosshairShowHide_Click;

                button_Link_Add.Click += Button_Link_Add_Click;
                button_Link_Remove.Click += Button_Link_Remove_Click;
            }
            catch { }
        }

        void Button_CrosshairShowHide_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vWindowMain.SwitchCrosshairVisibility(true);
            }
            catch { }
        }

        void Button_BrowserShowHide_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vWindowBrowser.Browser_Switch_Visibility();
            }
            catch { }
        }

        void Button_MoveUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button buttonsend = (Button)sender;
                if (buttonsend.Name == "button_GpuUp") { MoveStatsPosition(true, "GpuId"); }
                else if (buttonsend.Name == "button_CpuUp") { MoveStatsPosition(true, "CpuId"); }
                else if (buttonsend.Name == "button_MemUp") { MoveStatsPosition(true, "MemId"); }
                else if (buttonsend.Name == "button_NetUp") { MoveStatsPosition(true, "NetId"); }
                else if (buttonsend.Name == "button_FpsUp") { MoveStatsPosition(true, "FpsId"); }
                else if (buttonsend.Name == "button_FrametimeUp") { MoveStatsPosition(true, "FrametimeId"); }
                else if (buttonsend.Name == "button_AppUp") { MoveStatsPosition(true, "AppId"); }
                else if (buttonsend.Name == "button_BatUp") { MoveStatsPosition(true, "BatId"); }
                else if (buttonsend.Name == "button_TimeUp") { MoveStatsPosition(true, "TimeId"); }
                else if (buttonsend.Name == "button_CustomTextUp") { MoveStatsPosition(true, "CustomTextId"); }
                else if (buttonsend.Name == "button_MonUp") { MoveStatsPosition(true, "MonId"); }
                else if (buttonsend.Name == "button_FanUp") { MoveStatsPosition(true, "FanId"); }
            }
            catch { }
        }

        void Button_MoveDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button buttonsend = (Button)sender;
                if (buttonsend.Name == "button_GpuDown") { MoveStatsPosition(false, "GpuId"); }
                else if (buttonsend.Name == "button_CpuDown") { MoveStatsPosition(false, "CpuId"); }
                else if (buttonsend.Name == "button_MemDown") { MoveStatsPosition(false, "MemId"); }
                else if (buttonsend.Name == "button_NetDown") { MoveStatsPosition(false, "NetId"); }
                else if (buttonsend.Name == "button_FpsDown") { MoveStatsPosition(false, "FpsId"); }
                else if (buttonsend.Name == "button_FrametimeDown") { MoveStatsPosition(false, "FrametimeId"); }
                else if (buttonsend.Name == "button_AppDown") { MoveStatsPosition(false, "AppId"); }
                else if (buttonsend.Name == "button_BatDown") { MoveStatsPosition(false, "BatId"); }
                else if (buttonsend.Name == "button_TimeDown") { MoveStatsPosition(false, "TimeId"); }
                else if (buttonsend.Name == "button_CustomTextDown") { MoveStatsPosition(false, "CustomTextId"); }
                else if (buttonsend.Name == "button_MonDown") { MoveStatsPosition(false, "MonId"); }
                else if (buttonsend.Name == "button_FanDown") { MoveStatsPosition(false, "FanId"); }
            }
            catch { }
        }

        void UpdateStatsPositionText()
        {
            try
            {
                int totalId = vTotalStatsCount + 1;
                int AppId = SettingLoad(vConfigurationFpsOverlayer, "AppId", typeof(int)) + 1;
                int FpsId = SettingLoad(vConfigurationFpsOverlayer, "FpsId", typeof(int)) + 1;
                int FrametimeId = SettingLoad(vConfigurationFpsOverlayer, "FrametimeId", typeof(int)) + 1;
                int NetId = SettingLoad(vConfigurationFpsOverlayer, "NetId", typeof(int)) + 1;
                int CpuId = SettingLoad(vConfigurationFpsOverlayer, "CpuId", typeof(int)) + 1;
                int GpuId = SettingLoad(vConfigurationFpsOverlayer, "GpuId", typeof(int)) + 1;
                int MemId = SettingLoad(vConfigurationFpsOverlayer, "MemId", typeof(int)) + 1;
                int TimeId = SettingLoad(vConfigurationFpsOverlayer, "TimeId", typeof(int)) + 1;
                int CustomTextId = SettingLoad(vConfigurationFpsOverlayer, "CustomTextId", typeof(int)) + 1;
                int MonId = SettingLoad(vConfigurationFpsOverlayer, "MonId", typeof(int)) + 1;
                int BatId = SettingLoad(vConfigurationFpsOverlayer, "BatId", typeof(int)) + 1;
                int FanId = SettingLoad(vConfigurationFpsOverlayer, "FanId", typeof(int)) + 1;

                textblock_GpuPosition.Text = GpuId + "/" + totalId;
                textblock_CpuPosition.Text = CpuId + "/" + totalId;
                textblock_MemPosition.Text = MemId + "/" + totalId;
                textblock_NetPosition.Text = NetId + "/" + totalId;
                textblock_FpsPosition.Text = FpsId + "/" + totalId;
                textblock_FrametimePosition.Text = FrametimeId + "/" + totalId;
                textblock_AppPosition.Text = AppId + "/" + totalId;
                textblock_TimePosition.Text = TimeId + "/" + totalId;
                textblock_CustomTextPosition.Text = CustomTextId + "/" + totalId;
                textblock_MonPosition.Text = MonId + "/" + totalId;
                textblock_BatPosition.Text = BatId + "/" + totalId;
                textblock_FanPosition.Text = FanId + "/" + totalId;
            }
            catch { }
        }

        void MoveStatsPosition(bool moveUp, string targetName)
        {
            try
            {
                int AppId = SettingLoad(vConfigurationFpsOverlayer, "AppId", typeof(int));
                int FpsId = SettingLoad(vConfigurationFpsOverlayer, "FpsId", typeof(int));
                int FrametimeId = SettingLoad(vConfigurationFpsOverlayer, "FrametimeId", typeof(int));
                int NetId = SettingLoad(vConfigurationFpsOverlayer, "NetId", typeof(int));
                int CpuId = SettingLoad(vConfigurationFpsOverlayer, "CpuId", typeof(int));
                int GpuId = SettingLoad(vConfigurationFpsOverlayer, "GpuId", typeof(int));
                int MemId = SettingLoad(vConfigurationFpsOverlayer, "MemId", typeof(int));
                int TimeId = SettingLoad(vConfigurationFpsOverlayer, "TimeId", typeof(int));
                int CustomTextId = SettingLoad(vConfigurationFpsOverlayer, "CustomTextId", typeof(int));
                int MonId = SettingLoad(vConfigurationFpsOverlayer, "MonId", typeof(int));
                int BatId = SettingLoad(vConfigurationFpsOverlayer, "BatId", typeof(int));
                int FanId = SettingLoad(vConfigurationFpsOverlayer, "FanId", typeof(int));

                int newId = 0;
                int currentId = 0;
                if (!moveUp)
                {
                    if (targetName == "AppId") { currentId = AppId; newId = currentId + 1; }
                    else if (targetName == "FpsId") { currentId = FpsId; newId = currentId + 1; }
                    else if (targetName == "FrametimeId") { currentId = FrametimeId; newId = currentId + 1; }
                    else if (targetName == "NetId") { currentId = NetId; newId = currentId + 1; }
                    else if (targetName == "CpuId") { currentId = CpuId; newId = currentId + 1; }
                    else if (targetName == "GpuId") { currentId = GpuId; newId = currentId + 1; }
                    else if (targetName == "MemId") { currentId = MemId; newId = currentId + 1; }
                    else if (targetName == "TimeId") { currentId = TimeId; newId = currentId + 1; }
                    else if (targetName == "CustomTextId") { currentId = CustomTextId; newId = currentId + 1; }
                    else if (targetName == "MonId") { currentId = MonId; newId = currentId + 1; }
                    else if (targetName == "BatId") { currentId = BatId; newId = currentId + 1; }
                    else if (targetName == "FanId") { currentId = FanId; newId = currentId + 1; }
                }
                else
                {
                    if (targetName == "AppId") { currentId = AppId; newId = currentId - 1; }
                    else if (targetName == "FpsId") { currentId = FpsId; newId = currentId - 1; }
                    else if (targetName == "FrametimeId") { currentId = FrametimeId; newId = currentId - 1; }
                    else if (targetName == "NetId") { currentId = NetId; newId = currentId - 1; }
                    else if (targetName == "CpuId") { currentId = CpuId; newId = currentId - 1; }
                    else if (targetName == "GpuId") { currentId = GpuId; newId = currentId - 1; }
                    else if (targetName == "MemId") { currentId = MemId; newId = currentId - 1; }
                    else if (targetName == "TimeId") { currentId = TimeId; newId = currentId - 1; }
                    else if (targetName == "CustomTextId") { currentId = CustomTextId; newId = currentId - 1; }
                    else if (targetName == "MonId") { currentId = MonId; newId = currentId - 1; }
                    else if (targetName == "BatId") { currentId = BatId; newId = currentId - 1; }
                    else if (targetName == "FanId") { currentId = FanId; newId = currentId - 1; }
                }

                //Move current id
                if (newId <= vTotalStatsCount && newId >= 0)
                {
                    if (AppId == newId)
                    {
                        SettingSave(vConfigurationFpsOverlayer, "AppId", currentId.ToString());
                    }
                    else if (FpsId == newId)
                    {
                        SettingSave(vConfigurationFpsOverlayer, "FpsId", currentId.ToString());
                    }
                    else if (FrametimeId == newId)
                    {
                        SettingSave(vConfigurationFpsOverlayer, "FrametimeId", currentId.ToString());
                    }
                    else if (NetId == newId)
                    {
                        SettingSave(vConfigurationFpsOverlayer, "NetId", currentId.ToString());
                    }
                    else if (CpuId == newId)
                    {
                        SettingSave(vConfigurationFpsOverlayer, "CpuId", currentId.ToString());
                    }
                    else if (GpuId == newId)
                    {
                        SettingSave(vConfigurationFpsOverlayer, "GpuId", currentId.ToString());
                    }
                    else if (MemId == newId)
                    {
                        SettingSave(vConfigurationFpsOverlayer, "MemId", currentId.ToString());
                    }
                    else if (TimeId == newId)
                    {
                        SettingSave(vConfigurationFpsOverlayer, "TimeId", currentId.ToString());
                    }
                    else if (CustomTextId == newId)
                    {
                        SettingSave(vConfigurationFpsOverlayer, "CustomTextId", currentId.ToString());
                    }
                    else if (MonId == newId)
                    {
                        SettingSave(vConfigurationFpsOverlayer, "MonId", currentId.ToString());
                    }
                    else if (BatId == newId)
                    {
                        SettingSave(vConfigurationFpsOverlayer, "BatId", currentId.ToString());
                    }
                    else if (FanId == newId)
                    {
                        SettingSave(vConfigurationFpsOverlayer, "FanId", currentId.ToString());
                    }

                    //Save new id
                    SettingSave(vConfigurationFpsOverlayer, targetName, newId.ToString());
                    vWindowMain.UpdateFpsOverlayStyle();

                    //Update stats position text
                    UpdateStatsPositionText();
                }
            }
            catch { }
        }

        //Add link to list
        void Button_Link_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string textString = textbox_LinkString.Text;
                string placeholderString = (string)textbox_LinkString.GetValue(TextboxPlaceholder.PlaceholderProperty);
                Debug.WriteLine("Adding new link: " + textString);

                //Color brushes
                BrushConverter BrushConvert = new BrushConverter();
                Brush BrushInvalid = BrushConvert.ConvertFromString("#CD1A2B") as Brush;
                Brush BrushValid = BrushConvert.ConvertFromString("#1DB954") as Brush;

                //Check if the text is empty
                if (string.IsNullOrWhiteSpace(textString))
                {
                    textbox_LinkString.BorderBrush = BrushInvalid;
                    Debug.WriteLine("Please enter a link.");
                    return;
                }

                //Check if the text is place holder
                if (textString == placeholderString)
                {
                    textbox_LinkString.BorderBrush = BrushInvalid;
                    Debug.WriteLine("Please enter a link.");
                    return;
                }

                //Check if string is valid link
                textString = StringLinkCleanup(textString);
                if (!StringLinkValidate(textString))
                {
                    textbox_LinkString.BorderBrush = BrushInvalid;
                    Debug.WriteLine("Please enter proper link.");
                    return;
                }

                //Check if text already exists
                if (vFpsBrowserLinks.Any(x => x.String1.ToLower() == textString.ToLower()))
                {
                    textbox_LinkString.BorderBrush = BrushInvalid;
                    Debug.WriteLine("Link already exists.");
                    return;
                }

                //Clear text from the textbox
                textbox_LinkString.Text = placeholderString;
                textbox_LinkString.BorderBrush = BrushValid;

                //Add text string to the list
                ProfileShared profileShared = new ProfileShared();
                profileShared.String1 = textString;

                vFpsBrowserLinks.Add(profileShared);
                JsonSaveObject(vFpsBrowserLinks, @"Profiles\User\FpsBrowserLinks.json");
            }
            catch { }
        }

        //Remove link from list
        void Button_Link_Remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProfileShared selectedProfile = (ProfileShared)combobox_LinkString.SelectedItem;
                Debug.WriteLine("Removing link: " + selectedProfile.String1);

                //Remove mapping from list
                vFpsBrowserLinks.Remove(selectedProfile);

                //Save changes to Json file
                JsonSaveObject(vFpsBrowserLinks, @"Profiles\User\FpsBrowserLinks.json");

                //Select the default profile
                combobox_LinkString.SelectedIndex = 0;
            }
            catch { }
        }

        //Application Close Handler
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                this.Hide();
            }
            catch { }
        }
    }
}