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
                //Change the crosshair color
                string crosshairColor = Setting_Load(vConfigurationFpsOverlayer, "CrosshairColor").ToString();
                border_Crosshair.Background = new BrushConverter().ConvertFrom(crosshairColor) as SolidColorBrush;

                //Change the crosshair size
                int crosshairSize = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "CrosshairSize"));
                grid_CrosshairOverlayer.Width = crosshairSize;
                grid_CrosshairOverlayer.Height = crosshairSize;

                //Change the crosshair opacity
                grid_CrosshairOverlayer.Opacity = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "CrosshairOpacity"));
            }
            catch { }
        }
    }
}