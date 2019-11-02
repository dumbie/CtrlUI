using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FpsOverlayer
{
    public partial class WindowSettings : Window
    {
        //Application Launch
        public WindowSettings()
        {
            try
            {
                //Initialize Component
                InitializeComponent();

                //Start loading the application
                Loaded += Application_Loaded;
            }
            catch { }
        }

        //Application Loading
        void Application_Loaded(object sender, RoutedEventArgs args)
        {
            try
            {
                //Load settings
                Settings_Load();
                Settings_Save();

                //Register Interface Handlers
                RegisterInterfaceHandlers();

                Debug.WriteLine("Loaded application.");
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
            }
            catch { }
        }

        void Button_MoveUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button buttonsend = (Button)sender;
                if (buttonsend.Name == "button_GpuUp") { VideoCardUpDown(true, "GpuId"); }
                else if (buttonsend.Name == "button_CpuUp") { VideoCardUpDown(true, "CpuId"); }
                else if (buttonsend.Name == "button_MemUp") { VideoCardUpDown(true, "MemId"); }
                else if (buttonsend.Name == "button_NetUp") { VideoCardUpDown(true, "NetId"); }
                else if (buttonsend.Name == "button_FpsUp") { VideoCardUpDown(true, "FpsId"); }
                else if (buttonsend.Name == "button_AppUp") { VideoCardUpDown(true, "AppId"); }
            }
            catch { }
        }

        void Button_MoveDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button buttonsend = (Button)sender;
                if (buttonsend.Name == "button_GpuDown") { VideoCardUpDown(false, "GpuId"); }
                else if (buttonsend.Name == "button_CpuDown") { VideoCardUpDown(false, "CpuId"); }
                else if (buttonsend.Name == "button_MemDown") { VideoCardUpDown(false, "MemId"); }
                else if (buttonsend.Name == "button_NetDown") { VideoCardUpDown(false, "NetId"); }
                else if (buttonsend.Name == "button_FpsDown") { VideoCardUpDown(false, "FpsId"); }
                else if (buttonsend.Name == "button_AppDown") { VideoCardUpDown(false, "AppId"); }
            }
            catch { }
        }

        void VideoCardUpDown(bool moveUp, string targetName)
        {
            try
            {
                int AppId = Convert.ToInt32(ConfigurationManager.AppSettings["AppId"]);
                int FpsId = Convert.ToInt32(ConfigurationManager.AppSettings["FpsId"]);
                int NetId = Convert.ToInt32(ConfigurationManager.AppSettings["NetId"]);
                int CpuId = Convert.ToInt32(ConfigurationManager.AppSettings["CpuId"]);
                int GpuId = Convert.ToInt32(ConfigurationManager.AppSettings["GpuId"]);
                int MemId = Convert.ToInt32(ConfigurationManager.AppSettings["MemId"]);

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
                }
                else
                {
                    if (targetName == "AppId") { currentId = AppId; newId = currentId - 1; }
                    else if (targetName == "FpsId") { currentId = FpsId; newId = currentId - 1; }
                    else if (targetName == "NetId") { currentId = NetId; newId = currentId - 1; }
                    else if (targetName == "CpuId") { currentId = CpuId; newId = currentId - 1; }
                    else if (targetName == "GpuId") { currentId = GpuId; newId = currentId - 1; }
                    else if (targetName == "MemId") { currentId = MemId; newId = currentId - 1; }
                }

                if (newId <= 5 && newId >= 0)
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