using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static ArnoldVinkCode.AVTaskbarInformation;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        //Hide the stats visibility
        public void HideFpsOverlayVisibility()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_FpsOverlayer.Visibility = Visibility.Collapsed;
                });
            }
            catch { }
        }

        //Show the stats visibility
        public void ShowFpsOverlayVisibility()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_FpsOverlayer.Visibility = Visibility.Visible;
                });
            }
            catch { }
        }

        //Switch the stats visibility
        public void SwitchFpsOverlayVisibilityManual()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (grid_FpsOverlayer.Visibility == Visibility.Visible)
                    {
                        vManualHidden = true;
                        //Update the fps overlay position
                        UpdateFpsOverlayPosition(vTargetProcess.Name);
                    }
                    else
                    {
                        vManualHidden = false;
                        //Update the fps overlay position
                        UpdateFpsOverlayPosition(vTargetProcess.Name);
                    }
                });
            }
            catch { }
        }

        //Move the fps overlayer to the next position
        public void ChangeFpsOverlayPosition()
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
                if (vManualHidden || targetTextPosition == OverlayPosition.Hidden)
                {
                    HideFpsOverlayVisibility();
                    return;
                }
                else
                {
                    ShowFpsOverlayVisibility();
                }

                //Move fps to set position
                if (targetTextPosition == OverlayPosition.TopLeft)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
                        if (vTaskBarPosition == AppBarPosition.ABE_LEFT) { marginHorizontal += vTaskBarAdjustMargin; }
                        else if (vTaskBarPosition == AppBarPosition.ABE_TOP) { marginVertical += vTaskBarAdjustMargin; }
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
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Left;
                    });
                }
                else if (targetTextPosition == OverlayPosition.TopCenter)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
                        if (vTaskBarPosition == AppBarPosition.ABE_TOP) { marginVertical += vTaskBarAdjustMargin; }
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
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Center;
                    });
                }
                else if (targetTextPosition == OverlayPosition.TopRight)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
                        if (vTaskBarPosition == AppBarPosition.ABE_RIGHT) { marginHorizontal += vTaskBarAdjustMargin; }
                        else if (vTaskBarPosition == AppBarPosition.ABE_TOP) { marginVertical += vTaskBarAdjustMargin; }
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
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Right;
                    });
                }
                else if (targetTextPosition == OverlayPosition.MiddleRight)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
                        if (vTaskBarPosition == AppBarPosition.ABE_RIGHT) { marginHorizontal += vTaskBarAdjustMargin; }
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
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Right;
                    });
                }
                else if (targetTextPosition == OverlayPosition.BottomRight)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
                        if (vTaskBarPosition == AppBarPosition.ABE_RIGHT) { marginHorizontal += vTaskBarAdjustMargin; }
                        else if (vTaskBarPosition == AppBarPosition.ABE_BOTTOM) { marginVertical += vTaskBarAdjustMargin; }
                        marginVertical += vKeypadAdjustMargin;
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
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Right;
                    });
                }
                else if (targetTextPosition == OverlayPosition.BottomCenter)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
                        if (vTaskBarPosition == AppBarPosition.ABE_BOTTOM) { marginVertical += vTaskBarAdjustMargin; }
                        marginVertical += vKeypadAdjustMargin;
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
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Center;
                    });
                }
                else if (targetTextPosition == OverlayPosition.BottomLeft)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
                        if (vTaskBarPosition == AppBarPosition.ABE_LEFT) { marginHorizontal += vTaskBarAdjustMargin; }
                        else if (vTaskBarPosition == AppBarPosition.ABE_BOTTOM) { marginVertical += vTaskBarAdjustMargin; }
                        marginVertical += vKeypadAdjustMargin;
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
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Left;
                    });
                }
                else if (targetTextPosition == OverlayPosition.MiddleLeft)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        double marginHorizontal = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));
                        double marginVertical = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
                        if (vTaskBarPosition == AppBarPosition.ABE_LEFT) { marginHorizontal += vTaskBarAdjustMargin; }
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
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Left;
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
                if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "BatShowCategoryTitle")))
                {
                    vTitleBAT = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "BatCategoryTitle"));
                }
                else
                {
                    vTitleBAT = string.Empty;
                }

                //Load stats order identifier
                int TimeId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TimeId"));
                int MonId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "MonId"));
                int AppId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "AppId"));
                int FpsId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "FpsId"));
                int NetId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "NetId"));
                int CpuId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "CpuId"));
                int GpuId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "GpuId"));
                int MemId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "MemId"));
                int BatId = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "BatId"));

                //Update the stats text orientation and order
                if (Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TextDirection")) == 1)
                {
                    stackpanel_CurrentTime.SetValue(Grid.RowProperty, TimeId);
                    stackpanel_CurrentMon.SetValue(Grid.RowProperty, MonId);
                    stackpanel_CurrentApp.SetValue(Grid.RowProperty, AppId);
                    stackpanel_CurrentFps.SetValue(Grid.RowProperty, FpsId);
                    stackpanel_CurrentNet.SetValue(Grid.RowProperty, NetId);
                    stackpanel_CurrentCpu.SetValue(Grid.RowProperty, CpuId);
                    stackpanel_CurrentGpu.SetValue(Grid.RowProperty, GpuId);
                    stackpanel_CurrentMem.SetValue(Grid.RowProperty, MemId);
                    stackpanel_CurrentBat.SetValue(Grid.RowProperty, BatId);

                    stackpanel_CurrentMem.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentGpu.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentCpu.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentNet.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentFps.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentApp.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentTime.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentMon.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentBat.SetValue(Grid.ColumnProperty, 0);
                }
                else
                {
                    stackpanel_CurrentTime.SetValue(Grid.ColumnProperty, TimeId);
                    stackpanel_CurrentMon.SetValue(Grid.ColumnProperty, MonId);
                    stackpanel_CurrentApp.SetValue(Grid.ColumnProperty, AppId);
                    stackpanel_CurrentFps.SetValue(Grid.ColumnProperty, FpsId);
                    stackpanel_CurrentNet.SetValue(Grid.ColumnProperty, NetId);
                    stackpanel_CurrentCpu.SetValue(Grid.ColumnProperty, CpuId);
                    stackpanel_CurrentGpu.SetValue(Grid.ColumnProperty, GpuId);
                    stackpanel_CurrentMem.SetValue(Grid.ColumnProperty, MemId);
                    stackpanel_CurrentBat.SetValue(Grid.ColumnProperty, BatId);

                    stackpanel_CurrentMem.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentGpu.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentCpu.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentNet.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentFps.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentApp.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentTime.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentMon.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentBat.SetValue(Grid.RowProperty, 0);
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
                stackpanel_CurrentBat.Background = brushBackground;

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
                textblock_CurrentBat.FontSize = targetTextSize;
                textblock_CurrentBat.LineHeight = targetTextSize;
                textblock_CurrentBat.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;

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
                    textblock_CurrentBat.Foreground = brushForeground;
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
                    string ColorBat = Setting_Load(vConfigurationFpsOverlayer, "ColorBat").ToString();
                    textblock_CurrentMem.Foreground = new BrushConverter().ConvertFrom(ColorMem) as SolidColorBrush;
                    textblock_CurrentGpu.Foreground = new BrushConverter().ConvertFrom(ColorGpu) as SolidColorBrush;
                    textblock_CurrentCpu.Foreground = new BrushConverter().ConvertFrom(ColorCpu) as SolidColorBrush;
                    textblock_CurrentNet.Foreground = new BrushConverter().ConvertFrom(ColorNet) as SolidColorBrush;
                    textblock_CurrentFps.Foreground = new BrushConverter().ConvertFrom(ColorFps) as SolidColorBrush;
                    textblock_CurrentApp.Foreground = new BrushConverter().ConvertFrom(ColorApp) as SolidColorBrush;
                    textblock_CurrentTime.Foreground = new BrushConverter().ConvertFrom(ColorTime) as SolidColorBrush;
                    textblock_CurrentMon.Foreground = new BrushConverter().ConvertFrom(ColorMon) as SolidColorBrush;
                    textblock_CurrentBat.Foreground = new BrushConverter().ConvertFrom(ColorBat) as SolidColorBrush;
                }

                //Update the fps overlay position
                UpdateFpsOverlayPosition(vTargetProcess.Name);
            }
            catch { }
        }
    }
}