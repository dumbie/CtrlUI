using System;
using System.Windows;
using System.Windows.Media;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Settings;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        //Switch the crosshair visibility
        public void SwitchCrosshairVisibility()
        {
            try
            {
                if (grid_CrosshairOverlayer.Visibility == Visibility.Visible)
                {
                    grid_CrosshairOverlayer.Visibility = Visibility.Collapsed;
                }
                else
                {
                    grid_CrosshairOverlayer.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }

        //Show the crosshair visibility
        public void ShowCrosshairVisibility()
        {
            try
            {
                grid_CrosshairOverlayer.Visibility = Visibility.Visible;
            }
            catch { }
        }

        //Update the crosshair overlay style
        public void UpdateCrosshairOverlayStyle()
        {
            try
            {
                //Change the crosshair style
                int crosshairStyle = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "CrosshairStyle"));
                if (crosshairStyle == 0)
                {
                    crosshair_Dot.Visibility = Visibility.Visible;
                    crosshair_Cross.Visibility = Visibility.Collapsed;
                }
                else
                {
                    crosshair_Dot.Visibility = Visibility.Collapsed;
                    crosshair_Cross.Visibility = Visibility.Visible;
                }

                //Change the crosshair color
                string crosshairColor = Setting_Load(vConfigurationFpsOverlayer, "CrosshairColor").ToString();
                crosshair_Dot.Background = new BrushConverter().ConvertFrom(crosshairColor) as SolidColorBrush;
                crosshair_Cross_Vertical.Fill = new BrushConverter().ConvertFrom(crosshairColor) as SolidColorBrush;
                crosshair_Cross_Horizontal.Fill = new BrushConverter().ConvertFrom(crosshairColor) as SolidColorBrush;

                int crosshairSize = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "CrosshairSize"));
                int crosshairThickness = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "CrosshairThickness"));

                //Change the crosshair size - dot
                crosshair_Dot.Width = crosshairSize;
                crosshair_Dot.Height = crosshairSize;
                crosshair_Dot.CornerRadius = new CornerRadius(crosshairSize);

                //Change the crosshair size - cross
                crosshair_Cross_Vertical.Width = crosshairThickness;
                crosshair_Cross_Vertical.Height = crosshairSize;
                crosshair_Cross_Horizontal.Width = crosshairSize;
                crosshair_Cross_Horizontal.Height = crosshairThickness;

                //Change the crosshair opacity
                grid_CrosshairOverlayer.Opacity = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "CrosshairOpacity"));
            }
            catch { }
        }
    }
}