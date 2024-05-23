using ArnoldVinkCode;
using System.Windows;
using System.Windows.Media;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Enums;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        //Hide the crosshair visibility
        public void HideCrosshairVisibility()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    grid_CrosshairOverlayer.Visibility = Visibility.Collapsed;
                });
            }
            catch { }
        }

        //Show the crosshair visibility
        public void ShowCrosshairVisibility()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    vManualShownCrosshairOverlay = true;
                    grid_CrosshairOverlayer.Visibility = Visibility.Visible;
                });
            }
            catch { }
        }

        //Switch the crosshair visibility
        public void SwitchCrosshairVisibility(bool forceShow)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    if (grid_CrosshairOverlayer.Visibility == Visibility.Visible)
                    {
                        vManualHiddenCrosshairOverlay = true;
                        UpdateCrosshairOverlayPositionVisibility(vTargetProcess.ExeNameNoExt, forceShow);
                    }
                    else
                    {
                        vManualHiddenCrosshairOverlay = false;
                        UpdateCrosshairOverlayPositionVisibility(vTargetProcess.ExeNameNoExt, forceShow);
                    }
                });
            }
            catch { }
        }

        //Update window position and visibility
        public void UpdateCrosshairOverlayPositionVisibility(string processName, bool forceShow)
        {
            try
            {
                //Get target overlay position
                OverlayPosition targetOverlayPosition = GetFpsOverlayPosition(processName);

                //Hide or show crosshair overlay
                if (vManualHiddenCrosshairOverlay || targetOverlayPosition == OverlayPosition.Hidden)
                {
                    HideCrosshairVisibility();
                    return;
                }
                else if (forceShow || vManualShownCrosshairOverlay)
                {
                    ShowCrosshairVisibility();
                }

                //Change crosshair position
                int crosshairVerticalPosition = SettingLoad(vConfigurationFpsOverlayer, "CrosshairVerticalPosition", typeof(int));
                int crosshairHorizontalPosition = SettingLoad(vConfigurationFpsOverlayer, "CrosshairHorizontalPosition", typeof(int));
                AVActions.DispatcherInvoke(delegate
                {
                    grid_CrosshairOverlayer.Margin = new Thickness(crosshairHorizontalPosition, 0, 0, crosshairVerticalPosition);
                });
            }
            catch { }
        }

        //Update crosshair overlay style
        public void UpdateCrosshairOverlayStyle()
        {
            try
            {
                //Change the crosshair style
                crosshair_Dot.Visibility = Visibility.Collapsed;
                crosshair_Circle.Visibility = Visibility.Collapsed;
                crosshair_Squarebox.Visibility = Visibility.Collapsed;
                crosshair_CrossClosed.Visibility = Visibility.Collapsed;
                crosshair_CrossOpen.Visibility = Visibility.Collapsed;
                crosshair_LineHorizontal.Visibility = Visibility.Collapsed;
                int crosshairStyle = SettingLoad(vConfigurationFpsOverlayer, "CrosshairStyle", typeof(int));
                if (crosshairStyle == 0)
                {
                    crosshair_Dot.Visibility = Visibility.Visible;
                }
                else if (crosshairStyle == 1)
                {
                    crosshair_Circle.Visibility = Visibility.Visible;
                }
                else if (crosshairStyle == 2)
                {
                    crosshair_Squarebox.Visibility = Visibility.Visible;
                }
                else if (crosshairStyle == 3)
                {
                    crosshair_CrossClosed.Visibility = Visibility.Visible;
                }
                else if (crosshairStyle == 4)
                {
                    crosshair_CrossOpen.Visibility = Visibility.Visible;
                }
                else
                {
                    crosshair_LineHorizontal.Visibility = Visibility.Visible;
                }

                //Change the crosshair color
                string crosshairColor = SettingLoad(vConfigurationFpsOverlayer, "CrosshairColor", typeof(string));
                SolidColorBrush crosshairBrush = new BrushConverter().ConvertFrom(crosshairColor) as SolidColorBrush;
                crosshair_Dot.Background = crosshairBrush;
                crosshair_Circle.Stroke = crosshairBrush;
                crosshair_Squarebox.BorderBrush = crosshairBrush;
                crosshair_LineHorizontal.Fill = crosshairBrush;
                crosshair_CrossClosed_Vertical.Fill = crosshairBrush;
                crosshair_CrossClosed_Horizontal.Fill = crosshairBrush;
                crosshair_CrossOpen_VerticalTop.Fill = crosshairBrush;
                crosshair_CrossOpen_VerticalBottom.Fill = crosshairBrush;
                crosshair_CrossOpen_HorizontalLeft.Fill = crosshairBrush;
                crosshair_CrossOpen_HorizontalRight.Fill = crosshairBrush;

                int crosshairSize = SettingLoad(vConfigurationFpsOverlayer, "CrosshairSize", typeof(int));
                int crosshairThickness = SettingLoad(vConfigurationFpsOverlayer, "CrosshairThickness", typeof(int));

                //Change the crosshair size - dot
                crosshair_Dot.Width = crosshairSize;
                crosshair_Dot.Height = crosshairSize;
                crosshair_Dot.CornerRadius = new CornerRadius(crosshairSize);

                //Change the crosshair size - circle
                crosshair_Circle.Width = crosshairSize;
                crosshair_Circle.Height = crosshairSize;
                crosshair_Circle.StrokeThickness = crosshairThickness;

                //Change the crosshair size - squarebox
                crosshair_Squarebox.Width = crosshairSize;
                crosshair_Squarebox.Height = crosshairSize;
                crosshair_Squarebox.BorderThickness = new Thickness(crosshairThickness);

                //Change the crosshair size - cross closed
                crosshair_CrossClosed_Vertical.Width = crosshairThickness;
                crosshair_CrossClosed_Vertical.Height = crosshairSize;
                crosshair_CrossClosed_Horizontal.Width = crosshairSize;
                crosshair_CrossClosed_Horizontal.Height = crosshairThickness;

                //Change the crosshair size - cross open
                crosshair_CrossOpen_Center.Width = crosshairSize;
                crosshair_CrossOpen_Center.Height = crosshairSize;
                crosshair_CrossOpen_VerticalTop.Width = crosshairThickness;
                crosshair_CrossOpen_VerticalTop.Height = crosshairSize;
                crosshair_CrossOpen_VerticalBottom.Width = crosshairThickness;
                crosshair_CrossOpen_VerticalBottom.Height = crosshairSize;
                crosshair_CrossOpen_HorizontalLeft.Width = crosshairSize;
                crosshair_CrossOpen_HorizontalLeft.Height = crosshairThickness;
                crosshair_CrossOpen_HorizontalRight.Width = crosshairSize;
                crosshair_CrossOpen_HorizontalRight.Height = crosshairThickness;

                //Change the crosshair size - line
                crosshair_LineHorizontal.Width = crosshairSize;
                crosshair_LineHorizontal.Height = crosshairThickness;

                //Change crosshair opacity
                grid_CrosshairOverlayer.Opacity = SettingLoad(vConfigurationFpsOverlayer, "CrosshairOpacity", typeof(double));

                //Update crosshair position and visibility
                UpdateCrosshairOverlayPositionVisibility(vTargetProcess.ExeNameNoExt, false);
            }
            catch { }
        }
    }
}