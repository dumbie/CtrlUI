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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using static ArnoldVinkCode.AVInteropDll;
using static FpsOverlayer.AppTasks;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Classes;

namespace FpsOverlayer
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

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
                WindowSettings.Settings_Load_CtrlUI();

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
                SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
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

        //Get the current active screen
        public System.Windows.Forms.Screen GetActiveScreen()
        {
            try
            {
                //Get default monitor
                int MonitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);

                //Get the target screen
                if (MonitorNumber > 0)
                {
                    try
                    {
                        return System.Windows.Forms.Screen.AllScreens[MonitorNumber];
                    }
                    catch
                    {
                        return System.Windows.Forms.Screen.PrimaryScreen;
                    }
                }
            }
            catch { }
            return System.Windows.Forms.Screen.PrimaryScreen;
        }

        //Update the fps overlayer on resolution change
        public void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateFpsOverlayStyle();
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
                    App.vWindowSettings.SettingSave("TextSize", textSize.ToString());
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
                    App.vWindowSettings.SettingSave("TextSize", textSize.ToString());
                    UpdateFpsOverlayStyle();
                }
            }
            catch { }
        }

        //Move the fps overlayer to the next position
        public void ChangeWindowPosition()
        {
            try
            {
                int NextPosition = Convert.ToInt32(ConfigurationManager.AppSettings["TextPosition"]) + 1;
                if (NextPosition > 7)
                {
                    NextPosition = 0;
                }

                Debug.WriteLine("Changing text postion to: " + NextPosition);
                App.vWindowSettings.SettingSave("TextPosition", NextPosition.ToString());
                UpdateFpsOverlayStyle();
            }
            catch { }
        }

        //Update the stats window and text position
        public void UpdateWindowTextPosition(string processName)
        {
            try
            {
                //Get the current active screen
                System.Windows.Forms.Screen targetScreen = GetActiveScreen();

                //Get the screen resolution
                int ScreenWidth = targetScreen.Bounds.Width;
                int ScreenHeight = targetScreen.Bounds.Height;

                //Set the window size
                AVActions.ActionDispatcherInvoke(delegate
                {
                    this.Width = ScreenWidth;
                    this.Height = ScreenHeight;
                });

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
                else
                {
                    if (!vManualHidden)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            grid_FpsOverlayer.Visibility = Visibility.Visible;
                        });
                    }
                }

                //Move fps to set position
                if (targetTextPosition == 0) //Top left
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        this.Left = targetScreen.Bounds.Left + Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        this.Top = targetScreen.Bounds.Top + Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Top;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Left;
                    });
                }
                else if (targetTextPosition == 1) //Top center
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        this.Left = targetScreen.Bounds.Left + Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        this.Top = targetScreen.Bounds.Top + Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Top;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Center;
                    });
                }
                else if (targetTextPosition == 2) //Top right
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        this.Left = targetScreen.Bounds.Left - Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        this.Top = targetScreen.Bounds.Top + Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Top;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Right;
                    });
                }
                else if (targetTextPosition == 3) //Middle right
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        this.Left = targetScreen.Bounds.Left - Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        this.Top = targetScreen.Bounds.Top + Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Center;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Right;
                    });
                }
                else if (targetTextPosition == 4) //Bottom right
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        this.Left = targetScreen.Bounds.Left - Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        this.Top = targetScreen.Bounds.Top - Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Bottom;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Right;
                    });
                }
                else if (targetTextPosition == 5) //Bottom center
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        this.Left = targetScreen.Bounds.Left + Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        this.Top = targetScreen.Bounds.Top - Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Bottom;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Center;
                    });
                }
                else if (targetTextPosition == 6) //Bottom left
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        this.Left = targetScreen.Bounds.Left + Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        this.Top = targetScreen.Bounds.Top - Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Bottom;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Left;
                    });
                }
                else if (targetTextPosition == 7) //Middle left
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        this.Left = targetScreen.Bounds.Left + Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);
                        this.Top = targetScreen.Bounds.Top + Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Center;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Left;
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
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["CategoryTitles"]))
                {
                    vTitleGPU = "GPU";
                    vTitleCPU = "CPU";
                    vTitleMEM = "MEM";
                    vTitleNET = "NET";
                }
                else
                {
                    vTitleGPU = string.Empty;
                    vTitleCPU = string.Empty;
                    vTitleMEM = string.Empty;
                    vTitleNET = string.Empty;
                }

                //Update the stats text orientation and order
                if (Convert.ToInt32(ConfigurationManager.AppSettings["TextDirection"]) == 1)
                {
                    int TimeId = Convert.ToInt32(ConfigurationManager.AppSettings["TimeId"]);
                    stackpanel_CurrentTime.SetValue(Grid.RowProperty, TimeId);

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
                }
                else
                {
                    int TimeId = Convert.ToInt32(ConfigurationManager.AppSettings["TimeId"]);
                    stackpanel_CurrentTime.SetValue(Grid.ColumnProperty, TimeId);

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
                }

                //Update the stats background
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["DisplayBackground"]))
                {
                    string colorBackground = ConfigurationManager.AppSettings["ColorBackground"].ToString();
                    SolidColorBrush brushBackground = new BrushConverter().ConvertFrom(colorBackground) as SolidColorBrush;
                    stackpanel_CurrentMem.Background = brushBackground;
                    stackpanel_CurrentGpu.Background = brushBackground;
                    stackpanel_CurrentCpu.Background = brushBackground;
                    stackpanel_CurrentNet.Background = brushBackground;
                    stackpanel_CurrentFps.Background = brushBackground;
                    stackpanel_CurrentApp.Background = brushBackground;
                    stackpanel_CurrentTime.Background = brushBackground;
                }
                else
                {
                    SolidColorBrush brushBackground = new SolidColorBrush(Colors.Transparent);
                    stackpanel_CurrentMem.Background = brushBackground;
                    stackpanel_CurrentGpu.Background = brushBackground;
                    stackpanel_CurrentCpu.Background = brushBackground;
                    stackpanel_CurrentNet.Background = brushBackground;
                    stackpanel_CurrentFps.Background = brushBackground;
                    stackpanel_CurrentApp.Background = brushBackground;
                    stackpanel_CurrentTime.Background = brushBackground;
                }

                //Update the stats opacity
                grid_FpsOverlayer.Opacity = Convert.ToDouble(ConfigurationManager.AppSettings["DisplayOpacity"]);

                //Update the stats font family
                string textFontName = ConfigurationManager.AppSettings["TextFontName"].ToString();
                if (textFontName == "Segoe UI" || textFontName == "Verdana" || textFontName == "Consolas")
                {
                    this.FontFamily = new FontFamily(textFontName);
                }
                else
                {
                    try
                    {
                        string fontPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Assets\\Fonts\\" + textFontName + ".ttf";
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

                //Update the stats colors
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["TextColorSingle"]))
                {
                    string ColorSingle = ConfigurationManager.AppSettings["ColorSingle"].ToString();
                    SolidColorBrush brushBackground = new BrushConverter().ConvertFrom(ColorSingle) as SolidColorBrush;
                    textblock_CurrentMem.Foreground = brushBackground;
                    textblock_CurrentGpu.Foreground = brushBackground;
                    textblock_CurrentCpu.Foreground = brushBackground;
                    textblock_CurrentNet.Foreground = brushBackground;
                    textblock_CurrentFps.Foreground = brushBackground;
                    textblock_CurrentApp.Foreground = brushBackground;
                    textblock_CurrentTime.Foreground = brushBackground;
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
                    textblock_CurrentMem.Foreground = new BrushConverter().ConvertFrom(ColorMem) as SolidColorBrush;
                    textblock_CurrentGpu.Foreground = new BrushConverter().ConvertFrom(ColorGpu) as SolidColorBrush;
                    textblock_CurrentCpu.Foreground = new BrushConverter().ConvertFrom(ColorCpu) as SolidColorBrush;
                    textblock_CurrentNet.Foreground = new BrushConverter().ConvertFrom(ColorNet) as SolidColorBrush;
                    textblock_CurrentFps.Foreground = new BrushConverter().ConvertFrom(ColorFps) as SolidColorBrush;
                    textblock_CurrentApp.Foreground = new BrushConverter().ConvertFrom(ColorApp) as SolidColorBrush;
                    textblock_CurrentTime.Foreground = new BrushConverter().ConvertFrom(ColorTime) as SolidColorBrush;
                }

                //Update the stats text position
                UpdateWindowTextPosition(string.Empty);
            }
            catch { }
        }

        //Application Close Handler
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                Application_Exit();
            }
            catch { }
        }

        //Close the application
        void Application_Exit()
        {
            try
            {
                Debug.WriteLine("Exiting application.");
                AVActions.ActionDispatcherInvoke(delegate
                {
                    this.IsEnabled = false;
                });

                //Stop monitoring the hardware
                vHardwareComputer.Close();

                //Stop the background tasks
                TasksBackgroundStop();

                //Hide the visible tray icon
                TrayNotifyIcon.Visible = false;

                //Close the application
                Environment.Exit(0);
            }
            catch { }
        }
    }
}