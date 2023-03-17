using ArnoldVinkCode;
using System.Windows;
using System.Windows.Media;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        //Switch the crosshair visibility
        public void SwitchCrosshairVisibility()
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
                int crosshairStyle = SettingLoad(vConfigurationFpsOverlayer, "CrosshairStyle", typeof(int));
                if (crosshairStyle == 0)
                {
                    crosshair_Dot.Visibility = Visibility.Visible;
                    crosshair_Line.Visibility = Visibility.Collapsed;
                    crosshair_Cross.Visibility = Visibility.Collapsed;
                }
                else if (crosshairStyle == 1)
                {
                    crosshair_Dot.Visibility = Visibility.Collapsed;
                    crosshair_Line.Visibility = Visibility.Collapsed;
                    crosshair_Cross.Visibility = Visibility.Visible;
                }
                else
                {
                    crosshair_Dot.Visibility = Visibility.Collapsed;
                    crosshair_Line.Visibility = Visibility.Visible;
                    crosshair_Cross.Visibility = Visibility.Collapsed;
                }

                //Change the crosshair color
                string crosshairColor = SettingLoad(vConfigurationFpsOverlayer, "CrosshairColor", typeof(string));
                crosshair_Dot.Background = new BrushConverter().ConvertFrom(crosshairColor) as SolidColorBrush;
                crosshair_Line.Fill = new BrushConverter().ConvertFrom(crosshairColor) as SolidColorBrush;
                crosshair_Cross_Vertical.Fill = new BrushConverter().ConvertFrom(crosshairColor) as SolidColorBrush;
                crosshair_Cross_Horizontal.Fill = new BrushConverter().ConvertFrom(crosshairColor) as SolidColorBrush;

                int crosshairSize = SettingLoad(vConfigurationFpsOverlayer, "CrosshairSize", typeof(int));
                int crosshairThickness = SettingLoad(vConfigurationFpsOverlayer, "CrosshairThickness", typeof(int));

                //Change the crosshair size - dot
                crosshair_Dot.Width = crosshairSize;
                crosshair_Dot.Height = crosshairSize;
                crosshair_Dot.CornerRadius = new CornerRadius(crosshairSize);

                //Change the crosshair size - cross
                crosshair_Cross_Vertical.Width = crosshairThickness;
                crosshair_Cross_Vertical.Height = crosshairSize;
                crosshair_Cross_Horizontal.Width = crosshairSize;
                crosshair_Cross_Horizontal.Height = crosshairThickness;

                //Change the crosshair size - line
                crosshair_Line.Width = crosshairSize;
                crosshair_Line.Height = crosshairThickness;

                //Change the crosshair opacity
                grid_CrosshairOverlayer.Opacity = SettingLoad(vConfigurationFpsOverlayer, "CrosshairOpacity", typeof(double));

                //Change the vertical position
                int crosshairVerticalPosition = SettingLoad(vConfigurationFpsOverlayer, "CrosshairVerticalPosition", typeof(int));
                grid_CrosshairOverlayer.Margin = new Thickness(0, 0, 0, crosshairVerticalPosition);
            }
            catch { }
        }
    }
}