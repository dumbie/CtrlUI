using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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
                combobox_TextFontName.Items.Clear();

                //Add default fonts
                combobox_TextFontName.Items.Add("Segoe UI");
                combobox_TextFontName.Items.Add("Verdana");
                combobox_TextFontName.Items.Add("Consolas");

                //Add custom fonts
                DirectoryInfo directoryInfo = new DirectoryInfo("Assets\\Fonts\\");
                FileInfo[] fontFiles = directoryInfo.GetFiles("*.ttf", SearchOption.TopDirectoryOnly);
                foreach (FileInfo fontFile in fontFiles)
                {
                    combobox_TextFontName.Items.Add(Path.GetFileNameWithoutExtension(fontFile.Name));
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
            }
            catch { }
        }

        void Button_MoveUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button buttonsend = (Button)sender;
                if (buttonsend.Name == "button_GpuUp") { MoveStatsUpDown(true, "GpuId"); }
                else if (buttonsend.Name == "button_CpuUp") { MoveStatsUpDown(true, "CpuId"); }
                else if (buttonsend.Name == "button_MemUp") { MoveStatsUpDown(true, "MemId"); }
                else if (buttonsend.Name == "button_NetUp") { MoveStatsUpDown(true, "NetId"); }
                else if (buttonsend.Name == "button_FpsUp") { MoveStatsUpDown(true, "FpsId"); }
                else if (buttonsend.Name == "button_AppUp") { MoveStatsUpDown(true, "AppId"); }
                else if (buttonsend.Name == "button_TimeUp") { MoveStatsUpDown(true, "TimeId"); }
            }
            catch { }
        }

        void Button_MoveDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button buttonsend = (Button)sender;
                if (buttonsend.Name == "button_GpuDown") { MoveStatsUpDown(false, "GpuId"); }
                else if (buttonsend.Name == "button_CpuDown") { MoveStatsUpDown(false, "CpuId"); }
                else if (buttonsend.Name == "button_MemDown") { MoveStatsUpDown(false, "MemId"); }
                else if (buttonsend.Name == "button_NetDown") { MoveStatsUpDown(false, "NetId"); }
                else if (buttonsend.Name == "button_FpsDown") { MoveStatsUpDown(false, "FpsId"); }
                else if (buttonsend.Name == "button_AppDown") { MoveStatsUpDown(false, "AppId"); }
                else if (buttonsend.Name == "button_TimeDown") { MoveStatsUpDown(false, "TimeId"); }
            }
            catch { }
        }

        void MoveStatsUpDown(bool moveUp, string targetName)
        {
            try
            {
                int AppId = Convert.ToInt32(ConfigurationManager.AppSettings["AppId"]);
                int FpsId = Convert.ToInt32(ConfigurationManager.AppSettings["FpsId"]);
                int NetId = Convert.ToInt32(ConfigurationManager.AppSettings["NetId"]);
                int CpuId = Convert.ToInt32(ConfigurationManager.AppSettings["CpuId"]);
                int GpuId = Convert.ToInt32(ConfigurationManager.AppSettings["GpuId"]);
                int MemId = Convert.ToInt32(ConfigurationManager.AppSettings["MemId"]);
                int TimeId = Convert.ToInt32(ConfigurationManager.AppSettings["TimeId"]);

                int newId = 0;
                int currentId = 0;
                if (!moveUp)
                {
                    if (targetName == "AppId") { currentId = AppId; newId = currentId + 1; }
                    else if (targetName == "FpsId") { currentId = FpsId; newId = currentId + 1; }
                    else if (targetName == "NetId") { currentId = NetId; newId = currentId + 1; }
                    else if (targetName == "CpuId") { currentId = CpuId; newId = currentId + 1; }
                    else if (targetName == "GpuId") { currentId = GpuId; newId = currentId + 1; }
                    else if (targetName == "MemId") { currentId = MemId; newId = currentId + 1; }
                    else if (targetName == "TimeId") { currentId = TimeId; newId = currentId + 1; }
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
                }

                if (newId <= 6 && newId >= 0)
                {
                    //Move current id
                    if (AppId == newId)
                    {
                        SettingSave("AppId", currentId.ToString());
                    }
                    else if (FpsId == newId)
                    {
                        SettingSave("FpsId", currentId.ToString());
                    }
                    else if (NetId == newId)
                    {
                        SettingSave("NetId", currentId.ToString());
                    }
                    else if (CpuId == newId)
                    {
                        SettingSave("CpuId", currentId.ToString());
                    }
                    else if (GpuId == newId)
                    {
                        SettingSave("GpuId", currentId.ToString());
                    }
                    else if (MemId == newId)
                    {
                        SettingSave("MemId", currentId.ToString());
                    }
                    else if (TimeId == newId)
                    {
                        SettingSave("TimeId", currentId.ToString());
                    }

                    //Save new id
                    SettingSave(targetName, newId.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
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