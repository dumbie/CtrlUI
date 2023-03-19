using ArnoldVinkCode;
using Microsoft.Diagnostics.Tracing.StackSources;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVTaskbarInformation;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        //Hide the stats visibility
        public void HideFpsOverlayVisibility()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
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
                AVActions.DispatcherInvoke(delegate
                {
                    grid_FpsOverlayer.Visibility = Visibility.Visible;
                });
            }
            catch { }
        }

        //Switch the stats visibility
        public void SwitchFpsOverlayVisibility()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    if (grid_FpsOverlayer.Visibility == Visibility.Visible)
                    {
                        vManualHidden = true;
                        //Update the fps overlay position
                        UpdateFpsOverlayPosition(vTargetProcess.ExeNameNoExt);
                    }
                    else
                    {
                        vManualHidden = false;
                        //Update the fps overlay position
                        UpdateFpsOverlayPosition(vTargetProcess.ExeNameNoExt);
                    }
                });
            }
            catch { }
        }

        //Move window to next position
        public void ChangeFpsOverlayPosition()
        {
            try
            {
                int nextPosition = SettingLoad(vConfigurationFpsOverlayer, "TextPosition", typeof(int)) + 1;
                if (nextPosition > 7)
                {
                    nextPosition = 0;
                }

                Debug.WriteLine("Changing text postion to: " + nextPosition);
                SettingSave(vConfigurationFpsOverlayer, "TextPosition", nextPosition.ToString());

                AVActions.DispatcherInvoke(delegate
                {
                    UpdateFpsOverlayStyle();
                });
            }
            catch { }
        }

        //Get window position
        public OverlayPosition GetFpsOverlayPosition(string processName)
        {
            try
            {
                //Load the text position
                OverlayPosition targetTextPosition = (OverlayPosition)SettingLoad(vConfigurationFpsOverlayer, "TextPosition", typeof(int));
                if (!string.IsNullOrWhiteSpace(processName))
                {
                    ProfileShared FpsPositionProcessName = vFpsPositionProcessName.Where(x => x.String1.ToLower() == processName.ToLower()).FirstOrDefault();
                    if (FpsPositionProcessName != null)
                    {
                        Debug.WriteLine("Found fps position for: " + FpsPositionProcessName.String1 + " / " + FpsPositionProcessName.Int1);
                        targetTextPosition = (OverlayPosition)FpsPositionProcessName.Int1;
                    }
                }

                return targetTextPosition;
            }
            catch { }
            return OverlayPosition.TopLeft;
        }

        //Update window position
        public void UpdateFpsOverlayPosition(string processName)
        {
            try
            {
                //Get target overlay position
                OverlayPosition targetOverlayPosition = GetFpsOverlayPosition(processName);

                //Hide or show the fps overlayer
                if (vManualHidden || targetOverlayPosition == OverlayPosition.Hidden)
                {
                    HideFpsOverlayVisibility();
                    return;
                }
                else
                {
                    ShowFpsOverlayVisibility();
                }

                //Move fps to set position
                if (targetOverlayPosition == OverlayPosition.TopLeft)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        double marginHorizontal = SettingLoad(vConfigurationFpsOverlayer, "MarginHorizontal", typeof(double));
                        double marginVertical = SettingLoad(vConfigurationFpsOverlayer, "MarginVertical", typeof(double));
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
                        stackpanel_CurrentFrametime.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CustomText.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Left;
                    });
                }
                else if (targetOverlayPosition == OverlayPosition.TopCenter)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        double marginHorizontal = SettingLoad(vConfigurationFpsOverlayer, "MarginHorizontal", typeof(double));
                        double marginVertical = SettingLoad(vConfigurationFpsOverlayer, "MarginVertical", typeof(double));
                        if (vTaskBarPosition == AppBarPosition.ABE_TOP) { marginVertical += vTaskBarAdjustMargin; }
                        grid_FpsOverlayer.Margin = new Thickness(marginHorizontal, marginVertical, 0, 0);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Top;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentFrametime.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CustomText.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Center;
                    });
                }
                else if (targetOverlayPosition == OverlayPosition.TopRight)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        double marginHorizontal = SettingLoad(vConfigurationFpsOverlayer, "MarginHorizontal", typeof(double));
                        double marginVertical = SettingLoad(vConfigurationFpsOverlayer, "MarginVertical", typeof(double));
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
                        stackpanel_CurrentFrametime.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CustomText.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Right;
                    });
                }
                else if (targetOverlayPosition == OverlayPosition.MiddleRight)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        double marginHorizontal = SettingLoad(vConfigurationFpsOverlayer, "MarginHorizontal", typeof(double));
                        double marginVertical = SettingLoad(vConfigurationFpsOverlayer, "MarginVertical", typeof(double));
                        if (vTaskBarPosition == AppBarPosition.ABE_RIGHT) { marginHorizontal += vTaskBarAdjustMargin; }
                        grid_FpsOverlayer.Margin = new Thickness(0, marginVertical, marginHorizontal, 0);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Center;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentFrametime.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CustomText.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Right;
                    });
                }
                else if (targetOverlayPosition == OverlayPosition.BottomRight)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        double marginHorizontal = SettingLoad(vConfigurationFpsOverlayer, "MarginHorizontal", typeof(double));
                        double marginVertical = SettingLoad(vConfigurationFpsOverlayer, "MarginVertical", typeof(double));
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
                        stackpanel_CurrentFrametime.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CustomText.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Right;
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Right;
                    });
                }
                else if (targetOverlayPosition == OverlayPosition.BottomCenter)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        double marginHorizontal = SettingLoad(vConfigurationFpsOverlayer, "MarginHorizontal", typeof(double));
                        double marginVertical = SettingLoad(vConfigurationFpsOverlayer, "MarginVertical", typeof(double));
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
                        stackpanel_CurrentFrametime.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CustomText.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Center;
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Center;
                    });
                }
                else if (targetOverlayPosition == OverlayPosition.BottomLeft)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        double marginHorizontal = SettingLoad(vConfigurationFpsOverlayer, "MarginHorizontal", typeof(double));
                        double marginVertical = SettingLoad(vConfigurationFpsOverlayer, "MarginVertical", typeof(double));
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
                        stackpanel_CurrentFrametime.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CustomText.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentMon.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentBat.HorizontalAlignment = HorizontalAlignment.Left;
                    });
                }
                else if (targetOverlayPosition == OverlayPosition.MiddleLeft)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        double marginHorizontal = SettingLoad(vConfigurationFpsOverlayer, "MarginHorizontal", typeof(double));
                        double marginVertical = SettingLoad(vConfigurationFpsOverlayer, "MarginVertical", typeof(double));
                        if (vTaskBarPosition == AppBarPosition.ABE_LEFT) { marginHorizontal += vTaskBarAdjustMargin; }
                        grid_FpsOverlayer.Margin = new Thickness(marginHorizontal, marginVertical, 0, 0);
                        grid_FpsOverlayer.VerticalAlignment = VerticalAlignment.Center;
                        grid_FpsOverlayer.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentMem.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentGpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentCpu.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentNet.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentFps.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentFrametime.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentApp.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CurrentTime.HorizontalAlignment = HorizontalAlignment.Left;
                        stackpanel_CustomText.HorizontalAlignment = HorizontalAlignment.Left;
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
                if (SettingLoad(vConfigurationFpsOverlayer, "GpuShowCategoryTitle", typeof(bool)))
                {
                    vTitleGPU = SettingLoad(vConfigurationFpsOverlayer, "GpuCategoryTitle", typeof(string));
                }
                else
                {
                    vTitleGPU = string.Empty;
                }
                if (SettingLoad(vConfigurationFpsOverlayer, "CpuShowCategoryTitle", typeof(bool)))
                {
                    vTitleCPU = SettingLoad(vConfigurationFpsOverlayer, "CpuCategoryTitle", typeof(string));
                }
                else
                {
                    vTitleCPU = string.Empty;
                }
                if (SettingLoad(vConfigurationFpsOverlayer, "MemShowCategoryTitle", typeof(bool)))
                {
                    vTitleMEM = SettingLoad(vConfigurationFpsOverlayer, "MemCategoryTitle", typeof(string));
                }
                else
                {
                    vTitleMEM = string.Empty;
                }
                if (SettingLoad(vConfigurationFpsOverlayer, "NetShowCategoryTitle", typeof(bool)))
                {
                    vTitleNET = SettingLoad(vConfigurationFpsOverlayer, "NetCategoryTitle", typeof(string));
                }
                else
                {
                    vTitleNET = string.Empty;
                }
                if (SettingLoad(vConfigurationFpsOverlayer, "MonShowCategoryTitle", typeof(bool)))
                {
                    vTitleMON = SettingLoad(vConfigurationFpsOverlayer, "MonCategoryTitle", typeof(string));
                }
                else
                {
                    vTitleMON = string.Empty;
                }
                if (SettingLoad(vConfigurationFpsOverlayer, "FpsShowCategoryTitle", typeof(bool)))
                {
                    vTitleFPS = SettingLoad(vConfigurationFpsOverlayer, "FpsCategoryTitle", typeof(string));
                }
                else
                {
                    vTitleFPS = string.Empty;
                }
                if (SettingLoad(vConfigurationFpsOverlayer, "BatShowCategoryTitle", typeof(bool)))
                {
                    vTitleBAT = SettingLoad(vConfigurationFpsOverlayer, "BatCategoryTitle", typeof(string));
                }
                else
                {
                    vTitleBAT = string.Empty;
                }

                //Load stats order identifier
                int TimeId = SettingLoad(vConfigurationFpsOverlayer, "TimeId", typeof(int));
                int CustomTextId = SettingLoad(vConfigurationFpsOverlayer, "CustomTextId", typeof(int));
                int MonId = SettingLoad(vConfigurationFpsOverlayer, "MonId", typeof(int));
                int AppId = SettingLoad(vConfigurationFpsOverlayer, "AppId", typeof(int));
                int FpsId = SettingLoad(vConfigurationFpsOverlayer, "FpsId", typeof(int));
                int FrametimeId = SettingLoad(vConfigurationFpsOverlayer, "FrametimeId", typeof(int));
                int NetId = SettingLoad(vConfigurationFpsOverlayer, "NetId", typeof(int));
                int CpuId = SettingLoad(vConfigurationFpsOverlayer, "CpuId", typeof(int));
                int GpuId = SettingLoad(vConfigurationFpsOverlayer, "GpuId", typeof(int));
                int MemId = SettingLoad(vConfigurationFpsOverlayer, "MemId", typeof(int));
                int BatId = SettingLoad(vConfigurationFpsOverlayer, "BatId", typeof(int));

                //Update the stats text orientation and order
                if (SettingLoad(vConfigurationFpsOverlayer, "TextDirection", typeof(int)) == 1)
                {
                    //Reverse stats order when on bottom
                    if (SettingLoad(vConfigurationFpsOverlayer, "StatsFlipBottom", typeof(bool)))
                    {
                        OverlayPosition overlayPosition = GetFpsOverlayPosition(vTargetProcess.ExeNameNoExt);
                        if (overlayPosition == OverlayPosition.BottomLeft || overlayPosition == OverlayPosition.BottomCenter || overlayPosition == OverlayPosition.BottomRight)
                        {
                            TimeId = vTotalStatsCount - TimeId;
                            CustomTextId = vTotalStatsCount - CustomTextId;
                            MonId = vTotalStatsCount - MonId;
                            AppId = vTotalStatsCount - AppId;
                            FpsId = vTotalStatsCount - FpsId;
                            FrametimeId = vTotalStatsCount - FrametimeId;
                            NetId = vTotalStatsCount - NetId;
                            CpuId = vTotalStatsCount - CpuId;
                            GpuId = vTotalStatsCount - GpuId;
                            MemId = vTotalStatsCount - MemId;
                            BatId = vTotalStatsCount - BatId;
                        }
                    }

                    //Vertical text order
                    stackpanel_CurrentTime.SetValue(Grid.RowProperty, TimeId);
                    stackpanel_CustomText.SetValue(Grid.RowProperty, CustomTextId);
                    stackpanel_CurrentMon.SetValue(Grid.RowProperty, MonId);
                    stackpanel_CurrentApp.SetValue(Grid.RowProperty, AppId);
                    stackpanel_CurrentFps.SetValue(Grid.RowProperty, FpsId);
                    stackpanel_CurrentFrametime.SetValue(Grid.RowProperty, FrametimeId);
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
                    stackpanel_CurrentFrametime.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentApp.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentTime.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CustomText.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentMon.SetValue(Grid.ColumnProperty, 0);
                    stackpanel_CurrentBat.SetValue(Grid.ColumnProperty, 0);
                }
                else
                {
                    //Horizontal text order
                    stackpanel_CurrentTime.SetValue(Grid.ColumnProperty, TimeId);
                    stackpanel_CustomText.SetValue(Grid.ColumnProperty, CustomTextId);
                    stackpanel_CurrentMon.SetValue(Grid.ColumnProperty, MonId);
                    stackpanel_CurrentApp.SetValue(Grid.ColumnProperty, AppId);
                    stackpanel_CurrentFps.SetValue(Grid.ColumnProperty, FpsId);
                    stackpanel_CurrentFrametime.SetValue(Grid.ColumnProperty, FrametimeId);
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
                    stackpanel_CurrentFrametime.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentApp.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentTime.SetValue(Grid.RowProperty, 0);
                    stackpanel_CustomText.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentMon.SetValue(Grid.RowProperty, 0);
                    stackpanel_CurrentBat.SetValue(Grid.RowProperty, 0);
                }

                //Update the stats background
                SolidColorBrush brushBackground = null;
                if (SettingLoad(vConfigurationFpsOverlayer, "DisplayBackground", typeof(bool)))
                {
                    string colorBackground = SettingLoad(vConfigurationFpsOverlayer, "ColorBackground", typeof(string));
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
                stackpanel_CurrentFrametime.Background = brushBackground;
                stackpanel_CurrentApp.Background = brushBackground;
                stackpanel_CurrentTime.Background = brushBackground;
                stackpanel_CustomText.Background = brushBackground;
                stackpanel_CurrentMon.Background = brushBackground;
                stackpanel_CurrentBat.Background = brushBackground;

                //Update the stats opacity
                grid_FpsOverlayer.Opacity = SettingLoad(vConfigurationFpsOverlayer, "DisplayOpacity", typeof(double));

                //Adjust the application font family
                UpdateAppFontStyle();

                //Update the stats text size
                double targetTextSize = SettingLoad(vConfigurationFpsOverlayer, "TextSize", typeof(double));
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
                textblock_CustomText.FontSize = targetTextSize;
                textblock_CustomText.LineHeight = targetTextSize;
                textblock_CustomText.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
                textblock_CurrentMon.FontSize = targetTextSize;
                textblock_CurrentMon.LineHeight = targetTextSize;
                textblock_CurrentMon.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
                textblock_CurrentBat.FontSize = targetTextSize;
                textblock_CurrentBat.LineHeight = targetTextSize;
                textblock_CurrentBat.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;

                //Update the stats colors
                if (SettingLoad(vConfigurationFpsOverlayer, "TextColorSingle", typeof(bool)))
                {
                    string ColorSingle = SettingLoad(vConfigurationFpsOverlayer, "ColorSingle", typeof(string));
                    SolidColorBrush brushForeground = new BrushConverter().ConvertFrom(ColorSingle) as SolidColorBrush;
                    textblock_CurrentMem.Foreground = brushForeground;
                    textblock_CurrentGpu.Foreground = brushForeground;
                    textblock_CurrentCpu.Foreground = brushForeground;
                    textblock_CurrentNet.Foreground = brushForeground;
                    textblock_CurrentFps.Foreground = brushForeground;
                    polyline_Chart.Stroke = brushForeground;
                    textblock_CurrentApp.Foreground = brushForeground;
                    textblock_CurrentTime.Foreground = brushForeground;
                    textblock_CustomText.Foreground = brushForeground;
                    textblock_CurrentMon.Foreground = brushForeground;
                    textblock_CurrentBat.Foreground = brushForeground;
                }
                else
                {
                    string ColorMem = SettingLoad(vConfigurationFpsOverlayer, "ColorMem", typeof(string));
                    string ColorGpu = SettingLoad(vConfigurationFpsOverlayer, "ColorGpu", typeof(string));
                    string ColorCpu = SettingLoad(vConfigurationFpsOverlayer, "ColorCpu", typeof(string));
                    string ColorNet = SettingLoad(vConfigurationFpsOverlayer, "ColorNet", typeof(string));
                    string ColorFps = SettingLoad(vConfigurationFpsOverlayer, "ColorFps", typeof(string));
                    string ColorFrametime = SettingLoad(vConfigurationFpsOverlayer, "ColorFrametime", typeof(string));
                    string ColorApp = SettingLoad(vConfigurationFpsOverlayer, "ColorApp", typeof(string));
                    string ColorTime = SettingLoad(vConfigurationFpsOverlayer, "ColorTime", typeof(string));
                    string ColorCustomText = SettingLoad(vConfigurationFpsOverlayer, "ColorCustomText", typeof(string));
                    string ColorMon = SettingLoad(vConfigurationFpsOverlayer, "ColorMon", typeof(string));
                    string ColorBat = SettingLoad(vConfigurationFpsOverlayer, "ColorBat", typeof(string));
                    textblock_CurrentMem.Foreground = new BrushConverter().ConvertFrom(ColorMem) as SolidColorBrush;
                    textblock_CurrentGpu.Foreground = new BrushConverter().ConvertFrom(ColorGpu) as SolidColorBrush;
                    textblock_CurrentCpu.Foreground = new BrushConverter().ConvertFrom(ColorCpu) as SolidColorBrush;
                    textblock_CurrentNet.Foreground = new BrushConverter().ConvertFrom(ColorNet) as SolidColorBrush;
                    textblock_CurrentFps.Foreground = new BrushConverter().ConvertFrom(ColorFps) as SolidColorBrush;
                    polyline_Chart.Stroke = new BrushConverter().ConvertFrom(ColorFrametime) as SolidColorBrush;
                    textblock_CurrentApp.Foreground = new BrushConverter().ConvertFrom(ColorApp) as SolidColorBrush;
                    textblock_CurrentTime.Foreground = new BrushConverter().ConvertFrom(ColorTime) as SolidColorBrush;
                    textblock_CustomText.Foreground = new BrushConverter().ConvertFrom(ColorCustomText) as SolidColorBrush;
                    textblock_CurrentMon.Foreground = new BrushConverter().ConvertFrom(ColorMon) as SolidColorBrush;
                    textblock_CurrentBat.Foreground = new BrushConverter().ConvertFrom(ColorBat) as SolidColorBrush;
                }

                //Update frametime graph size
                stackpanel_CurrentFrametime.Height = SettingLoad(vConfigurationFpsOverlayer, "FrametimeHeight", typeof(double));
                stackpanel_CurrentFrametime.Width = SettingLoad(vConfigurationFpsOverlayer, "FrametimeWidth", typeof(double));

                //Update the fps overlay position
                UpdateFpsOverlayPosition(vTargetProcess.ExeNameNoExt);
            }
            catch { }
        }
    }
}