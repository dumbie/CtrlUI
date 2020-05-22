using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVInteropDll;
using static FpsOverlayer.AppTasks;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;

namespace FpsOverlayer
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Variables
        public static IntPtr vInteropWindowHandle = IntPtr.Zero;

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get application interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Update the window style
                UpdateWindowStyle();

                //Check application settings
                App.vWindowSettings.Settings_Check();
                Settings_Load_CtrlUI(ref vConfigurationCtrlUI);

                //Update the window position
                UpdateWindowPosition();

                //Update the fps overlay style
                UpdateFpsOverlayStyle();

                //Create tray icon
                Application_CreateTrayMenu();

                //Load Json profiles
                JsonFunctions.JsonLoadProfile(ref vFpsPositionProcessName, "FpsPositionProcessName");

                //Start process monitoring
                StartMonitorProcess();

                //Start fps monitoring
                StartMonitorFps();

                //Start hardware monitoring
                StartMonitorHardware();

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

                //Enable the socket server
                EnableSocketServer();
            }
            catch { }
        }

        //Enable the socket server
        private void EnableSocketServer()
        {
            try
            {
                int SocketServerPort = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["ServerPort"].Value) + 3;

                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", SocketServerPort);
                vArnoldVinkSockets.vTcpClientTimeout = 250;
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
            }
            catch { }
        }

        //Update the window position on resolution change
        public void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Update the window position
                UpdateWindowPosition();
            }
            catch { }
        }

        //Update the window style
        void UpdateWindowStyle()
        {
            try
            {
                //Set the window style
                IntPtr UpdatedStyle = new IntPtr((uint)WindowStyles.WS_VISIBLE);
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, UpdatedStyle);

                //Set the window style ex
                IntPtr UpdatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE | WindowStylesEx.WS_EX_TRANSPARENT));
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_EXSTYLE, UpdatedExStyle);

                //Set the window as top most
                SetWindowPos(vInteropWindowHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)(WindowSWP.NOMOVE | WindowSWP.NOSIZE));

                Debug.WriteLine("The window style has been updated.");
            }
            catch { }
        }

        //Update the window position
        public void UpdateWindowPosition()
        {
            try
            {
                //Load current CtrlUI settings
                Settings_Load_CtrlUI(ref vConfigurationCtrlUI);

                //Get the current active screen
                int monitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);
                DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);

                //Move and resize the window
                WindowMove(vInteropWindowHandle, displayMonitorSettings.BoundsLeft, displayMonitorSettings.BoundsTop);
                WindowResize(vInteropWindowHandle, displayMonitorSettings.WidthNative, displayMonitorSettings.HeightNative);
            }
            catch { }
        }

        //Change the font size up or down
        public void ChangeFontSize(bool sizeUp)
        {
            try
            {
                if (sizeUp)
                {
                    int textSize = Convert.ToInt32(ConfigurationManager.AppSettings["TextSize"]) + 1;
                    if (textSize > 120)
                    {
                        return;
                    }

                    Debug.WriteLine("Changing text size to: " + textSize);
                    SettingSave(vConfigurationApplication, "TextSize", textSize.ToString());
                    UpdateFpsOverlayStyle();
                }
                else
                {
                    int textSize = Convert.ToInt32(ConfigurationManager.AppSettings["TextSize"]) - 1;
                    if (textSize < 8)
                    {
                        return;
                    }

                    Debug.WriteLine("Changing text size to: " + textSize);
                    SettingSave(vConfigurationApplication, "TextSize", textSize.ToString());
                    UpdateFpsOverlayStyle();
                }
            }
            catch { }
        }

        //Move the fps overlayer to the next position
        public async Task ChangeWindowPosition()
        {
            try
            {
                int nextPosition = Convert.ToInt32(ConfigurationManager.AppSettings["TextPosition"]) + 1;
                if (nextPosition > 7)
                {
                    nextPosition = 0;
                }

                Debug.WriteLine("Changing text postion to: " + nextPosition);
                SettingSave(vConfigurationApplication, "TextPosition", nextPosition.ToString());
                UpdateFpsOverlayStyle();
                await App.vWindowSettings.NotifyDirectXInputSettingChanged("TextPosition");
            }
            catch { }
        }

        //Update the window position
        public void UpdateFpsOverlayPosition(string processName)
        {
            try
            {
                //Load the text position
                int targetTextPosition = Convert.ToInt32(ConfigurationManager.AppSettings["TextPosition"]);
                if (!string.IsNullOrWhiteSpace(processName))
                {
                    ProfileShared FpsPositionProcessName = vFpsPositionProcessName.Where(x => x.String1.ToLower() == processName.ToLower()).FirstOrDefault();
                    if (FpsPositionProcessName != null)
                    {
                        Debug.WriteLine("Found fps position for: " + FpsPositionProcessName.String1 + " / " + FpsPositionProcessName.Int1);
                        targetTextPosition = (int)FpsPositionProcessName.Int1;
                    }
                }

                //Hide or show the fps overlayer
                if (targetTextPosition == 8)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_FpsOverlayer.Visibility = Visibility.Collapsed;
                    });
                    return;
                }
                else if (!vManualHidden)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_FpsOverlayer.Visibility = Visibility.Visible;
                    });
                }

                //Move fps to set position
                if (targetTextPosition == 0) //Top left
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        double marginVertical = Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.Margin = new Thickness(marginHorizontal, marginVertical, 0, 0);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Top;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Left;
                    });
                }
                else if (targetTextPosition == 1) //Top center
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        double marginVertical = Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.Margin = new Thickness(marginHorizontal, marginVertical, 0, 0);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Top;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Center;
                    });
                }
                else if (targetTextPosition == 2) //Top right
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        double marginVertical = Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.Margin = new Thickness(0, marginVertical, marginHorizontal, 0);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Top;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Right;
                    });
                }
                else if (targetTextPosition == 3) //Middle right
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        double marginVertical = Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.Margin = new Thickness(0, marginVertical, marginHorizontal, 0);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Center;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Right;
                    });
                }
                else if (targetTextPosition == 4) //Bottom right
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        double marginVertical = Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.Margin = new Thickness(0, 0, marginHorizontal, marginVertical);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Bottom;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Right;
                    });
                }
                else if (targetTextPosition == 5) //Bottom center
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        double marginVertical = Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.Margin = new Thickness(marginHorizontal, 0, 0, marginVertical);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Bottom;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Center;
                    });
                }
                else if (targetTextPosition == 6) //Bottom left
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        double marginVertical = Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.Margin = new Thickness(marginHorizontal, 0, 0, marginVertical);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Bottom;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Left;
                    });
                }
                else if (targetTextPosition == 7) //Middle left
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        double marginVertical = Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.Margin = new Thickness(marginHorizontal, marginVertical, 0, 0);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Center;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Left;
                    });
                }
            }
            catch { }
        }

        //Update the fps overlay style
        public void UpdateFpsOverlayStyle()
        {
            try
            {
                //Update the stats titles
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowCategoryTitle"]))
                {
                    vTitleGPU = Convert.ToString(ConfigurationManager.AppSettings["GpuCategoryTitle"]);
                }
                else
                {
                    vTitleGPU = string.Empty;
                }
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowCategoryTitle"]))
                {
                    vTitleCPU = Convert.ToString(ConfigurationManager.AppSettings["CpuCategoryTitle"]);
                }
                else
                {
                    vTitleCPU = string.Empty;
                }
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["MemShowCategoryTitle"]))
                {
                    vTitleMEM = Convert.ToString(ConfigurationManager.AppSettings["MemCategoryTitle"]);
                }
                else
                {
                    vTitleMEM = string.Empty;
                }
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["NetShowCategoryTitle"]))
                {
                    vTitleNET = Convert.ToString(ConfigurationManager.AppSettings["NetCategoryTitle"]);
                }
                else
                {
                    vTitleNET = string.Empty;
                }
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["MonShowCategoryTitle"]))
                {
                    vTitleMON = Convert.ToString(ConfigurationManager.AppSettings["MonCategoryTitle"]);
                }
                else
                {
                    vTitleMON = string.Empty;
                }
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["FpsShowCategoryTitle"]))
                {
                    vTitleFPS = Convert.ToString(ConfigurationManager.AppSettings["FpsCategoryTitle"]);
                }
                else
                {
                    vTitleFPS = string.Empty;
                }

                //Update the stats text orientation and order
                if (Convert.ToInt32(ConfigurationManager.AppSettings["TextDirection"]) == 1)
                {
                    int TimeId = Convert.ToInt32(ConfigurationManager.AppSettings["TimeId"]);
                    stackpanel_CurrentTime.SetValue(Grid.RowProperty, TimeId);

                    int MonId = Convert.ToInt32(ConfigurationManager.AppSettings["MonId"]);
                    stackpanel_CurrentMon.SetValue(Grid.RowProperty, MonId);

                    int AppId = Convert.ToInt32(ConfigurationManager.AppSettings["AppId"]);
                    stackpanel_CurrentApp.SetValue(Grid.RowProperty, AppId);

                    int FpsId = Convert.ToInt32(ConfigurationManager.AppSettings["FpsId"]);
                    stackpanel_CurrentFps.SetValue(Grid.RowProperty, FpsId);

                    int NetId = Convert.ToInt32(ConfigurationManager.AppSettings["NetId"]);
                    stackpanel_CurrentNet.SetValue(Grid.RowProperty, NetId);

                    int CpuId = Convert.ToInt32(ConfigurationManager.AppSettings["CpuId"]);
                    stackpanel_CurrentCpu.SetValue(Grid.RowProperty, CpuId);

                    int GpuId = Convert.ToInt32(ConfigurationManager.AppSettings["GpuId"]);
                    stackpanel_CurrentGpu.SetValue(Grid.RowProperty, GpuId);

                    int MemId = Convert.ToInt32(ConfigurationManager.AppSettings["MemId"]);
                    stackpanel_CurrentMem.SetValue(Grid.RowProperty, MemId);

                    stackpanel_CurrentMem.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentGpu.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentCpu.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentNet.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentFps.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentApp.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentTime.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentMon.SetValue(Grid.ColumnProperty, 0);
                }
                else
                {
                    int TimeId = Convert.ToInt32(ConfigurationManager.AppSettings["TimeId"]);
                    stackpanel_CurrentTime.SetValue(Grid.ColumnProperty, TimeId);

                    int MonId = Convert.ToInt32(ConfigurationManager.AppSettings["MonId"]);
                    stackpanel_CurrentMon.SetValue(Grid.ColumnProperty, MonId);

                    int AppId = Convert.ToInt32(ConfigurationManager.AppSettings["AppId"]);
                    stackpanel_CurrentApp.SetValue(Grid.ColumnProperty, AppId);

                    int FpsId = Convert.ToInt32(ConfigurationManager.AppSettings["FpsId"]);
                    stackpanel_CurrentFps.SetValue(Grid.ColumnProperty, FpsId);

                    int NetId = Convert.ToInt32(ConfigurationManager.AppSettings["NetId"]);
                    stackpanel_CurrentNet.SetValue(Grid.ColumnProperty, NetId);

                    int CpuId = Convert.ToInt32(ConfigurationManager.AppSettings["CpuId"]);
                    stackpanel_CurrentCpu.SetValue(Grid.ColumnProperty, CpuId);

                    int GpuId = Convert.ToInt32(ConfigurationManager.AppSettings["GpuId"]);
                    stackpanel_CurrentGpu.SetValue(Grid.ColumnProperty, GpuId);

                    int MemId = Convert.ToInt32(ConfigurationManager.AppSettings["MemId"]);
                    stackpanel_CurrentMem.SetValue(Grid.ColumnProperty, MemId);

                    stackpanel_CurrentMem.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentGpu.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentCpu.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentNet.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentFps.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentApp.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentTime.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentMon.SetValue(Grid.RowProperty, 0);
                }

                //Update the stats background
                SolidColorBrush brushBackground = null;
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["DisplayBackground"]))
                {
                    string colorBackground = ConfigurationManager.AppSettings["ColorBackground"].ToString();
                    brushBackground = new BrushConverter().ConvertFrom(colorBackground) as SolidColorBrush;
                }
                else
                {
                    brushBackground = new SolidColorBrush(Colors.Transparent);
                }
                stackpanel_CurrentMem.Background = brushBackground;
                stackpanel_CurrentGpu.Background = brushBackground;
                stackpanel_CurrentCpu.Background = brushBackground;
                stackpanel_CurrentNet.Background = brushBackground;
                stackpanel_CurrentFps.Background = brushBackground;
                stackpanel_CurrentApp.Background = brushBackground;
                stackpanel_CurrentTime.Background = brushBackground;
                stackpanel_CurrentMon.Background = brushBackground;

                //Update the stats opacity
                grid_FpsOverlayer.Opacity = Convert.ToDouble(ConfigurationManager.AppSettings["DisplayOpacity"]);

                //Update the stats font family
                string InterfaceFontStyleName = ConfigurationManager.AppSettings["InterfaceFontStyleName"].ToString();
                if (InterfaceFontStyleName == "Segoe UI" || InterfaceFontStyleName == "Verdana" || InterfaceFontStyleName == "Consolas")
                {
                    this.FontFamily = new FontFamily(InterfaceFontStyleName);
                }
                else
                {
                    try
                    {
                        string fontPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Assets/Fonts/" + InterfaceFontStyleName + ".ttf";
                        ICollection<FontFamily> fontFamilies = Fonts.GetFontFamilies(fontPath);
                        this.FontFamily = fontFamilies.First();
                    }
                    catch
                    {
                        Debug.WriteLine("Failed loading the custom font.");
                    }
                }

                //Update the stats text size
                double targetTextSize = Convert.ToDouble(ConfigurationManager.AppSettings["TextSize"]);
                textblock_CurrentMem.FontSize = targetTextSize;
                textblock_CurrentMem.LineHeight = targetTextSize;
                textblock_CurrentMem.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
                textblock_CurrentGpu.FontSize = targetTextSize;
                textblock_CurrentGpu.LineHeight = targetTextSize;
                textblock_CurrentGpu.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
                textblock_CurrentCpu.FontSize = targetTextSize;
                textblock_CurrentCpu.LineHeight = targetTextSize;
                textblock_CurrentCpu.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
                textblock_CurrentNet.FontSize = targetTextSize;
                textblock_CurrentNet.LineHeight = targetTextSize;
                textblock_CurrentNet.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
                textblock_CurrentFps.FontSize = targetTextSize;
                textblock_CurrentFps.LineHeight = targetTextSize;
                textblock_CurrentFps.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
                textblock_CurrentApp.FontSize = targetTextSize;
                textblock_CurrentApp.LineHeight = targetTextSize;
                textblock_CurrentApp.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
                textblock_CurrentTime.FontSize = targetTextSize;
                textblock_CurrentTime.LineHeight = targetTextSize;
                textblock_CurrentTime.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
                textblock_CurrentMon.FontSize = targetTextSize;
                textblock_CurrentMon.LineHeight = targetTextSize;
                textblock_CurrentMon.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;

                //Update the stats colors
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["TextColorSingle"]))
                {
                    string ColorSingle = ConfigurationManager.AppSettings["ColorSingle"].ToString();
                    SolidColorBrush brushForeground = new BrushConverter().ConvertFrom(ColorSingle) as SolidColorBrush;
                    textblock_CurrentMem.Foreground = brushForeground;
                    textblock_CurrentGpu.Foreground = brushForeground;
                    textblock_CurrentCpu.Foreground = brushForeground;
                    textblock_CurrentNet.Foreground = brushForeground;
                    textblock_CurrentFps.Foreground = brushForeground;
                    textblock_CurrentApp.Foreground = brushForeground;
                    textblock_CurrentTime.Foreground = brushForeground;
                    textblock_CurrentMon.Foreground = brushForeground;
                }
                else
                {
                    string ColorMem = ConfigurationManager.AppSettings["ColorMem"].ToString();
                    string ColorGpu = ConfigurationManager.AppSettings["ColorGpu"].ToString();
                    string ColorCpu = ConfigurationManager.AppSettings["ColorCpu"].ToString();
                    string ColorNet = ConfigurationManager.AppSettings["ColorNet"].ToString();
                    string ColorFps = ConfigurationManager.AppSettings["ColorFps"].ToString();
                    string ColorApp = ConfigurationManager.AppSettings["ColorApp"].ToString();
                    string ColorTime = ConfigurationManager.AppSettings["ColorTime"].ToString();
                    string ColorMon = ConfigurationManager.AppSettings["ColorMon"].ToString();
                    textblock_CurrentMem.Foreground = new BrushConverter().ConvertFrom(ColorMem) as SolidColorBrush;
                    textblock_CurrentGpu.Foreground = new BrushConverter().ConvertFrom(ColorGpu) as SolidColorBrush;
                    textblock_CurrentCpu.Foreground = new BrushConverter().ConvertFrom(ColorCpu) as SolidColorBrush;
                    textblock_CurrentNet.Foreground = new BrushConverter().ConvertFrom(ColorNet) as SolidColorBrush;
                    textblock_CurrentFps.Foreground = new BrushConverter().ConvertFrom(ColorFps) as SolidColorBrush;
                    textblock_CurrentApp.Foreground = new BrushConverter().ConvertFrom(ColorApp) as SolidColorBrush;
                    textblock_CurrentTime.Foreground = new BrushConverter().ConvertFrom(ColorTime) as SolidColorBrush;
                    textblock_CurrentMon.Foreground = new BrushConverter().ConvertFrom(ColorMon) as SolidColorBrush;
                }

                //Update the fps overlay position
                UpdateFpsOverlayPosition(vTargetProcess.Name);
            }
            catch { }
        }

        //Application Close Handler
        protected override async void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                await Application_Exit();
            }
            catch { }
        }

        //Close the application
        async Task Application_Exit()
        {
            try
            {
                Debug.WriteLine("Exiting application.");
                AVActions.ActionDispatcherInvoke(delegate
                {
                    this.Opacity = 0.80;
                    this.IsEnabled = false;
                });

                //Stop monitoring the hardware
                vHardwareComputer.Close();

                //Stop the background tasks
                await TasksBackgroundStop();

                //Disable the socket server
                if (vArnoldVinkSockets != null)
                {
                    await vArnoldVinkSockets.SocketServerDisable();
                }

                //Hide the visible tray icon
                TrayNotifyIcon.Visible = false;

                //Close the application
                Environment.Exit(0);
            }
            catch { }
        }
    }
}