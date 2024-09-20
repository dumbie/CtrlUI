﻿using ArnoldVinkCode;
using System.Windows;
using System.Windows.Media;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowCrosshair
    {
        //Switch the crosshair visibility
        public void SwitchCrosshairVisibility(bool forceShow)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    if (grid_CrosshairOverlayer.Visibility == Visibility.Visible)
                    {
                        grid_CrosshairOverlayer.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        grid_CrosshairOverlayer.Visibility = Visibility.Visible;
                    }
                });
            }
            catch { }
        }

        //Update crosshair overlay style
        public void UpdateCrosshairOverlayStyle()
        {
            try
            {
                //Change crosshair style
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

                //Change crosshair color
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

                //Change crosshair size - dot
                crosshair_Dot.Width = crosshairSize;
                crosshair_Dot.Height = crosshairSize;
                crosshair_Dot.CornerRadius = new CornerRadius(crosshairSize);

                //Change crosshair size - circle
                crosshair_Circle.Width = crosshairSize;
                crosshair_Circle.Height = crosshairSize;
                crosshair_Circle.StrokeThickness = crosshairThickness;

                //Change crosshair size - squarebox
                crosshair_Squarebox.Width = crosshairSize;
                crosshair_Squarebox.Height = crosshairSize;
                crosshair_Squarebox.BorderThickness = new Thickness(crosshairThickness);

                //Change crosshair size - cross closed
                crosshair_CrossClosed_Vertical.Width = crosshairThickness;
                crosshair_CrossClosed_Vertical.Height = crosshairSize;
                crosshair_CrossClosed_Horizontal.Width = crosshairSize;
                crosshair_CrossClosed_Horizontal.Height = crosshairThickness;

                //Change crosshair size - cross open
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

                //Change crosshair size - line
                crosshair_LineHorizontal.Width = crosshairSize;
                crosshair_LineHorizontal.Height = crosshairThickness;

                //Change crosshair opacity
                grid_CrosshairOverlayer.Opacity = SettingLoad(vConfigurationFpsOverlayer, "CrosshairOpacity", typeof(double));

                //Change crosshair position
                int crosshairVerticalPosition = SettingLoad(vConfigurationFpsOverlayer, "CrosshairVerticalPosition", typeof(int));
                int crosshairHorizontalPosition = SettingLoad(vConfigurationFpsOverlayer, "CrosshairHorizontalPosition", typeof(int));
                grid_CrosshairOverlayer.Margin = new Thickness(crosshairHorizontalPosition, 0, 0, crosshairVerticalPosition);
            }
            catch { }
        }
    }
}