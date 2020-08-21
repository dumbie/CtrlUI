using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static FpsOverlayer.AppVariables;
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
                DirectoryInfo directoryInfo = new DirectoryInfo("Assets/Fonts");
                FileInfo[] fontFiles = directoryInfo.GetFiles("*.ttf", SearchOption.TopDirectoryOnly);
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

                button_TimeUp.Click += Button_MoveUp_Click;
                button_TimeDown.Click += Button_MoveDown_Click;

                button_MonUp.Click += Button_MoveUp_Click;
                button_MonDown.Click += Button_MoveDown_Click;
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
                else if (buttonsend.Name == "button_TimeUp") { MoveStatsPosition(true, "TimeId"); }
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
                else if (buttonsend.Name == "button_TimeDown") { MoveStatsPosition(false, "TimeId"); }
                else if (buttonsend.Name == "button_MonDown") { MoveStatsPosition(false, "MonId"); }
            }
            catch { }
        }

        void UpdateStatsPositionText()
        {
            try
            {
                int totalId = 8;
                int AppId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "AppId")) + 1;
                int FpsId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "FpsId")) + 1;
                int NetId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "NetId")) + 1;
                int CpuId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "CpuId")) + 1;
                int GpuId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "GpuId")) + 1;
                int MemId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "MemId")) + 1;
                int TimeId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TimeId")) + 1;
                int MonId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "MonId")) + 1;

                textblock_GpuPosition.Text = GpuId + "/" + totalId;
                textblock_CpuPosition.Text = CpuId + "/" + totalId;
                textblock_MemPosition.Text = MemId + "/" + totalId;
                textblock_NetPosition.Text = NetId + "/" + totalId;
                textblock_FpsPosition.Text = FpsId + "/" + totalId;
                textblock_AppPosition.Text = AppId + "/" + totalId;
                textblock_TimePosition.Text = TimeId + "/" + totalId;
                textblock_MonPosition.Text = MonId + "/" + totalId;
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
                int MonId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "MonId"));

                int newId = 0;
                int currentId = 0;
                int totalId = 7;
                if (!moveUp)
                {
                    if (targetName == "AppId") { currentId = AppId; newId = currentId + 1; }
                    else if (targetName == "FpsId") { currentId = FpsId; newId = currentId + 1; }
                    else if (targetName == "NetId") { currentId = NetId; newId = currentId + 1; }
                    else if (targetName == "CpuId") { currentId = CpuId; newId = currentId + 1; }
                    else if (targetName == "GpuId") { currentId = GpuId; newId = currentId + 1; }
                    else if (targetName == "MemId") { currentId = MemId; newId = currentId + 1; }
                    else if (targetName == "TimeId") { currentId = TimeId; newId = currentId + 1; }
                    else if (targetName == "MonId") { currentId = MonId; newId = currentId + 1; }
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
                    else if (targetName == "MonId") { currentId = MonId; newId = currentId - 1; }
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
                    else if (MonId == newId)
                    {
                        Setting_Save(vConfigurationFpsOverlayer, "MonId", currentId.ToString());
                    }

                    //Save new id
                    Setting_Save(vConfigurationFpsOverlayer, targetName, newId.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();

                    //Update stats position text
                    UpdateStatsPositionText();
                }
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