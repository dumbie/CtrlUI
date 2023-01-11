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
using static FpsOverlayer.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.JsonFunctions;
using static LibraryShared.Settings;

namespace FpsOverlayer
{
    public partial class WindowSettings : Window
    {
        //Window Initialize
        public WindowSettings() { InitializeComponent(); }

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Load available fonts
                LoadAvailableFonts();

                //Check application settings
                Settings_Load();
                Settings_Save();

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
                vWindowMain.SwitchCrosshairVisibility();
            }
            catch { }
        }

        async void Button_BrowserShowHide_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await vWindowBrowser.SwitchBrowserVisibility();
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
                else if (buttonsend.Name == "button_AppUp") { MoveStatsPosition(true, "AppId"); }
                else if (buttonsend.Name == "button_BatUp") { MoveStatsPosition(true, "BatId"); }
                else if (buttonsend.Name == "button_TimeUp") { MoveStatsPosition(true, "TimeId"); }
                else if (buttonsend.Name == "button_CustomTextUp") { MoveStatsPosition(true, "CustomTextId"); }
                else if (buttonsend.Name == "button_MonUp") { MoveStatsPosition(true, "MonId"); }
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
                else if (buttonsend.Name == "button_AppDown") { MoveStatsPosition(false, "AppId"); }
                else if (buttonsend.Name == "button_BatDown") { MoveStatsPosition(false, "BatId"); }
                else if (buttonsend.Name == "button_TimeDown") { MoveStatsPosition(false, "TimeId"); }
                else if (buttonsend.Name == "button_CustomTextDown") { MoveStatsPosition(false, "CustomTextId"); }
                else if (buttonsend.Name == "button_MonDown") { MoveStatsPosition(false, "MonId"); }
            }
            catch { }
        }

        void UpdateStatsPositionText()
        {
            try
            {
                int totalId = 10;
                int AppId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "AppId")) + 1;
                int FpsId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "FpsId")) + 1;
                int NetId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "NetId")) + 1;
                int CpuId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "CpuId")) + 1;
                int GpuId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "GpuId")) + 1;
                int MemId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "MemId")) + 1;
                int TimeId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TimeId")) + 1;
                int CustomTextId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "CustomTextId")) + 1;
                int MonId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "MonId")) + 1;
                int BatId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "BatId")) + 1;

                textblock_GpuPosition.Text = GpuId + "/" + totalId;
                textblock_CpuPosition.Text = CpuId + "/" + totalId;
                textblock_MemPosition.Text = MemId + "/" + totalId;
                textblock_NetPosition.Text = NetId + "/" + totalId;
                textblock_FpsPosition.Text = FpsId + "/" + totalId;
                textblock_AppPosition.Text = AppId + "/" + totalId;
                textblock_TimePosition.Text = TimeId + "/" + totalId;
                textblock_CustomTextPosition.Text = CustomTextId + "/" + totalId;
                textblock_MonPosition.Text = MonId + "/" + totalId;
                textblock_BatPosition.Text = BatId + "/" + totalId;
            }
            catch { }
        }

        void MoveStatsPosition(bool moveUp, string targetName)
        {
            try
            {
                int AppId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "AppId"));
                int FpsId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "FpsId"));
                int NetId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "NetId"));
                int CpuId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "CpuId"));
                int GpuId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "GpuId"));
                int MemId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "MemId"));
                int TimeId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TimeId"));
                int CustomTextId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "CustomTextId"));
                int MonId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "MonId"));
                int BatId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "BatId"));

                int newId = 0;
                int currentId = 0;
                int totalId = 9;
                if (!moveUp)
                {
                    if (targetName == "AppId") { currentId = AppId; newId = currentId + 1; }
                    else if (targetName == "FpsId") { currentId = FpsId; newId = currentId + 1; }
                    else if (targetName == "NetId") { currentId = NetId; newId = currentId + 1; }
                    else if (targetName == "CpuId") { currentId = CpuId; newId = currentId + 1; }
                    else if (targetName == "GpuId") { currentId = GpuId; newId = currentId + 1; }
                    else if (targetName == "MemId") { currentId = MemId; newId = currentId + 1; }
                    else if (targetName == "TimeId") { currentId = TimeId; newId = currentId + 1; }
                    else if (targetName == "CustomTextId") { currentId = CustomTextId; newId = currentId + 1; }
                    else if (targetName == "MonId") { currentId = MonId; newId = currentId + 1; }
                    else if (targetName == "BatId") { currentId = BatId; newId = currentId + 1; }
                }
                else
                {
                    if (targetName == "AppId") { currentId = AppId; newId = currentId - 1; }
                    else if (targetName == "FpsId") { currentId = FpsId; newId = currentId - 1; }
                    else if (targetName == "NetId") { currentId = NetId; newId = currentId - 1; }
                    else if (targetName == "CpuId") { currentId = CpuId; newId = currentId - 1; }
                    else if (targetName == "GpuId") { currentId = GpuId; newId = currentId - 1; }
                    else if (targetName == "MemId") { currentId = MemId; newId = currentId - 1; }
                    else if (targetName == "TimeId") { currentId = TimeId; newId = currentId - 1; }
                    else if (targetName == "CustomTextId") { currentId = CustomTextId; newId = currentId - 1; }
                    else if (targetName == "MonId") { currentId = MonId; newId = currentId - 1; }
                    else if (targetName == "BatId") { currentId = BatId; newId = currentId - 1; }
                }

                //Move current id
                if (newId <= totalId && newId >= 0)
                {
                    if (AppId == newId)
                    {
                        Setting_Save(vConfigurationFpsOverlayer, "AppId", currentId.ToString());
                    }
                    else if (FpsId == newId)
                    {
                        Setting_Save(vConfigurationFpsOverlayer, "FpsId", currentId.ToString());
                    }
                    else if (NetId == newId)
                    {
                        Setting_Save(vConfigurationFpsOverlayer, "NetId", currentId.ToString());
                    }
                    else if (CpuId == newId)
                    {
                        Setting_Save(vConfigurationFpsOverlayer, "CpuId", currentId.ToString());
                    }
                    else if (GpuId == newId)
                    {
                        Setting_Save(vConfigurationFpsOverlayer, "GpuId", currentId.ToString());
                    }
                    else if (MemId == newId)
                    {
                        Setting_Save(vConfigurationFpsOverlayer, "MemId", currentId.ToString());
                    }
                    else if (TimeId == newId)
                    {
                        Setting_Save(vConfigurationFpsOverlayer, "TimeId", currentId.ToString());
                    }
                    else if (CustomTextId == newId)
                    {
                        Setting_Save(vConfigurationFpsOverlayer, "CustomTextId", currentId.ToString());
                    }
                    else if (MonId == newId)
                    {
                        Setting_Save(vConfigurationFpsOverlayer, "MonId", currentId.ToString());
                    }
                    else if (BatId == newId)
                    {
                        Setting_Save(vConfigurationFpsOverlayer, "BatId", currentId.ToString());
                    }

                    //Save new id
                    Setting_Save(vConfigurationFpsOverlayer, targetName, newId.ToString());
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
                textString = StringLinkFixup(textString);
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
                JsonSaveObject(vFpsBrowserLinks, @"User\FpsBrowserLinks");
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
                JsonSaveObject(vFpsBrowserLinks, @"User\FpsBrowserLinks");

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