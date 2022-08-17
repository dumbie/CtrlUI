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

        //Update the crosshair overlay style
        public void UpdateCrosshairOverlayStyle()
        {
            try
            {
                //Change the crosshair color
                string ColorCrosshair = Setting_Load(vConfigurationFpsOverlayer, "ColorCrosshair").ToString();
                border_Crosshair.Background = new BrushConverter().ConvertFrom(ColorCrosshair) as SolidColorBrush;

                //Change the crosshair size
                grid_CrosshairOverlayer.Width = 4;
                grid_CrosshairOverlayer.Height = 4;

                //Change the crosshair opacity
                grid_CrosshairOverlayer.Opacity = 0.90;
            }
            catch { }
        }
    }
}