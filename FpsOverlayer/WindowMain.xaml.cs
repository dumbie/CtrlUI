using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static LibraryShared.Enums;
using static LibraryShared.JsonFunctions;
using static LibraryShared.Settings;

namespace FpsOverlayer
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Variables
        private IntPtr vInteropWindowHandle = IntPtr.Zero;
        public bool vWindowVisible = false;

        //Window Initialized
        protected override async void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Update the window style
                UpdateWindowStyle();

                //Check application settings
                App.vWindowSettings.Settings_Check();

                //Update the window position
                UpdateWindowPosition();

                //Update the fps overlay style
                UpdateFpsOverlayStyle();

                //Create tray icon
                Application_CreateTrayMenu();

                //Load Json profiles
                JsonLoadProfile(ref vFpsPositionProcessName, "FpsPositionProcessName");

                //Start process monitoring
                StartMonitorProcess();

                //Start fps monitoring
                StartMonitorFps();

                //Start hardware monitoring
                StartMonitorHardware();

                //Enable the socket server
                await EnableSocketServer();

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            }
            catch { }
        }

        //Hide the window
        public new void Hide()
        {
            try
            {
                //Update the window visibility
                UpdateWindowVisibility(false);
            }
            catch { }
        }

        //Show the window
        public new void Show()
        {
            try
            {
                //Update the window visibility
                UpdateWindowVisibility(true);
            }
            catch { }
        }

        //Enable the socket server
        private async Task EnableSocketServer()
        {
            try
            {
                int SocketServerPort = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "ServerPort")) + 2;

                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", SocketServerPort, true, false);
                vArnoldVinkSockets.vSocketTimeout = 250;
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
                await vArnoldVinkSockets.SocketServerEnable();
            }
            catch { }
        }

        //Update the window position on resolution change
        public async void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Wait for change to complete
                await Task.Delay(500);

                //Update the window position
                UpdateWindowPosition();
            }
            catch { }
        }

        //Switch the window visibility
        public void SwitchWindowVisibilityManual()
        {
            try
            {
                if (vWindowVisible)
                {
                    UpdateWindowVisibility(false);
                    vManualHidden = true;
                }
                else
                {
                    UpdateWindowVisibility(true);
                    vManualHidden = false;
                }
            }
            catch { }
        }

        //Update the window visibility
        public void UpdateWindowVisibility(bool visible)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (visible)
                    {
                        //Create and show the window
                        base.Show();

                        //Update the window style (focus workaround)
                        UpdateWindowStyle();

                        this.Title = "Fps Overlayer (Visible)";
                        vWindowVisible = true;
                        Debug.WriteLine("Showing the window.");
                    }
                    else
                    {
                        //Update the window style
                        IntPtr updatedStyle = IntPtr.Zero;
                        SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                        this.Title = "Fps Overlayer (Hidden)";
                        vWindowVisible = false;
                        Debug.WriteLine("Hiding the window.");
                    }
                });
            }
            catch { }
        }

        //Update the window style (focus workaround)
        void UpdateWindowStyle()
        {
            try
            {
                //Set the window style
                IntPtr updatedStyle = new IntPtr((uint)WindowStyles.WS_VISIBLE);
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                //Set the window style ex
                IntPtr updatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE | WindowStylesEx.WS_EX_TRANSPARENT));
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_EXSTYLE, updatedExStyle);

                //Set the window as top most
                SetWindowPos(vInteropWindowHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)(WindowSWP.NOMOVE | WindowSWP.NOSIZE | WindowSWP.FRAMECHANGED | WindowSWP.NOCOPYBITS));
            }
            catch { }
        }

        //Update the window position
        public void UpdateWindowPosition()
        {
            try
            {
                //Load current CtrlUI settings
                vConfigurationCtrlUI = Settings_Load_CtrlUI();

                //Get the current active screen
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));
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
                    int textSize = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TextSize")) + 1;
                    if (textSize > 120)
                    {
                        return;
                    }

                    Debug.WriteLine("Changing text size to: " + textSize);
                    Setting_Save(vConfigurationFpsOverlayer, "TextSize", textSize.ToString());
                    UpdateFpsOverlayStyle();
                }
                else
                {
                    int textSize = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TextSize")) - 1;
                    if (textSize < 8)
                    {
                        return;
                    }

                    Debug.WriteLine("Changing text size to: " + textSize);
                    Setting_Save(vConfigurationFpsOverlayer, "TextSize", textSize.ToString());
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
                int nextPosition = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TextPosition")) + 1;
                if (nextPosition > 7)
                {
                    nextPosition = 0;
                }

                Debug.WriteLine("Changing text postion to: " + nextPosition);
                Setting_Save(vConfigurationFpsOverlayer, "TextPosition", nextPosition.ToString());
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
                OverlayPosition targetTextPosition = (OverlayPosition)Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TextPosition"));
                if (!string.IsNullOrWhiteSpace(processName))
                {
                    ProfileShared FpsPositionProcessName = vFpsPositionProcessName.Where(x => x.String1.ToLower() == processName.ToLower()).FirstOrDefault();
                    if (FpsPositionProcessName != null)
                    {
                        Debug.WriteLine("Found fps position for: " + FpsPositionProcessName.String1 + " / " + FpsPositionProcessName.Int1);
                        targetTextPosition = (OverlayPosition)FpsPositionProcessName.Int1;
                    }
                }

                //Hide or show the fps overlayer
                if (targetTextPosition == OverlayPosition.Hidden)
                {
                    UpdateWindowVisibility(false);
                    return;
                }
                else if (!vManualHidden)
                {
                    UpdateWindowVisibility(true);
                }

                //Move fps to set position
                if (targetTextPosition == OverlayPosition.TopLeft)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
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
                else if (targetTextPosition == OverlayPosition.TopCenter)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
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
                else if (targetTextPosition == OverlayPosition.TopRight)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
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
                else if (targetTextPosition == OverlayPosition.MiddleRight)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
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
                else if (targetTextPosition == OverlayPosition.BottomRight)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
                        marginVertical += vKeypadBottomMargin;
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
                else if (targetTextPosition == OverlayPosition.BottomCenter)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
                        marginVertical += vKeypadBottomMargin;
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
                else if (targetTextPosition == OverlayPosition.BottomLeft)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
                        marginVertical += vKeypadBottomMargin;
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
                else if (targetTextPosition == OverlayPosition.MiddleLeft)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
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
                if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowCategoryTitle")))
                {
                    vTitleGPU = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "GpuCategoryTitle"));
                }
                else
                {
                    vTitleGPU = string.Empty;
                }
                if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowCategoryTitle")))
                {
                    vTitleCPU = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "CpuCategoryTitle"));
                }
                else
                {
                    vTitleCPU = string.Empty;
                }
                if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MemShowCategoryTitle")))
                {
                    vTitleMEM = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "MemCategoryTitle"));
                }
                else
                {
                    vTitleMEM = string.Empty;
                }
                if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "NetShowCategoryTitle")))
                {
                    vTitleNET = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "NetCategoryTitle"));
                }
                else
                {
                    vTitleNET = string.Empty;
                }
                if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MonShowCategoryTitle")))
                {
                    vTitleMON = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "MonCategoryTitle"));
                }
                else
                {
                    vTitleMON = string.Empty;
                }
                if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "FpsShowCategoryTitle")))
                {
                    vTitleFPS = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "FpsCategoryTitle"));
                }
                else
                {
                    vTitleFPS = string.Empty;
                }

                //Update the stats text orientation and order
                if (Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TextDirection")) == 1)
                {
                    int TimeId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TimeId"));
                    stackpanel_CurrentTime.SetValue(Grid.RowProperty, TimeId);

                    int MonId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "MonId"));
                    stackpanel_CurrentMon.SetValue(Grid.RowProperty, MonId);

                    int AppId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "AppId"));
                    stackpanel_CurrentApp.SetValue(Grid.RowProperty, AppId);

                    int FpsId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "FpsId"));
                    stackpanel_CurrentFps.SetValue(Grid.RowProperty, FpsId);

                    int NetId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "NetId"));
                    stackpanel_CurrentNet.SetValue(Grid.RowProperty, NetId);

                    int CpuId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "CpuId"));
                    stackpanel_CurrentCpu.SetValue(Grid.RowProperty, CpuId);

                    int GpuId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "GpuId"));
                    stackpanel_CurrentGpu.SetValue(Grid.RowProperty, GpuId);

                    int MemId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "MemId"));
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
                    int TimeId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TimeId"));
                    stackpanel_CurrentTime.SetValue(Grid.ColumnProperty, TimeId);

                    int MonId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "MonId"));
                    stackpanel_CurrentMon.SetValue(Grid.ColumnProperty, MonId);

                    int AppId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "AppId"));
                    stackpanel_CurrentApp.SetValue(Grid.ColumnProperty, AppId);

                    int FpsId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "FpsId"));
                    stackpanel_CurrentFps.SetValue(Grid.ColumnProperty, FpsId);

                    int NetId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "NetId"));
                    stackpanel_CurrentNet.SetValue(Grid.ColumnProperty, NetId);

                    int CpuId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "CpuId"));
                    stackpanel_CurrentCpu.SetValue(Grid.ColumnProperty, CpuId);

                    int GpuId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "GpuId"));
                    stackpanel_CurrentGpu.SetValue(Grid.ColumnProperty, GpuId);

                    int MemId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "MemId"));
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
                if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "DisplayBackground")))
                {
                    string colorBackground = Setting_Load(vConfigurationFpsOverlayer, "ColorBackground").ToString();
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
                grid_FpsOverlayer.Opacity = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "DisplayOpacity"));

                //Adjust the application font family
                UpdateAppFontStyle();

                //Update the stats text size
                double targetTextSize = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "TextSize"));
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
                if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "TextColorSingle")))
                {
                    string ColorSingle = Setting_Load(vConfigurationFpsOverlayer, "ColorSingle").ToString();
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
                    string ColorMem = Setting_Load(vConfigurationFpsOverlayer, "ColorMem").ToString();
                    string ColorGpu = Setting_Load(vConfigurationFpsOverlayer, "ColorGpu").ToString();
                    string ColorCpu = Setting_Load(vConfigurationFpsOverlayer, "ColorCpu").ToString();
                    string ColorNet = Setting_Load(vConfigurationFpsOverlayer, "ColorNet").ToString();
                    string ColorFps = Setting_Load(vConfigurationFpsOverlayer, "ColorFps").ToString();
                    string ColorApp = Setting_Load(vConfigurationFpsOverlayer, "ColorApp").ToString();
                    string ColorTime = Setting_Load(vConfigurationFpsOverlayer, "ColorTime").ToString();
                    string ColorMon = Setting_Load(vConfigurationFpsOverlayer, "ColorMon").ToString();
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

        //Adjust the application font family
        void UpdateAppFontStyle()
        {
            try
            {
                string interfaceFontStyleName = Setting_Load(vConfigurationFpsOverlayer, "InterfaceFontStyleName").ToString();
                if (interfaceFontStyleName == "Segoe UI" || interfaceFontStyleName == "Verdana" || interfaceFontStyleName == "Consolas" || interfaceFontStyleName == "Arial")
                {
                    this.FontFamily = new FontFamily(interfaceFontStyleName);
                }
                else
                {
                    string fontPathUser = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Assets/User/Fonts/" + interfaceFontStyleName + ".ttf";
                    string fontPathDefault = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Assets/Default/Fonts/" + interfaceFontStyleName + ".ttf";
                    if (File.Exists(fontPathUser))
                    {
                        ICollection<FontFamily> fontFamilies = Fonts.GetFontFamilies(fontPathUser);
                        this.FontFamily = fontFamilies.FirstOrDefault();
                    }
                    else if (File.Exists(fontPathDefault))
                    {
                        ICollection<FontFamily> fontFamilies = Fonts.GetFontFamilies(fontPathDefault);
                        this.FontFamily = fontFamilies.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed setting application font: " + ex.Message);
            }
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