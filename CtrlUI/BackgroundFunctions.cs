using Microsoft.Win32;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Unload the current background media
        void UnloadBackgroundMedia()
        {
            try
            {
                grid_Video_Background.Source = null;
            }
            catch { }
        }

        //Update the application background media
        void UpdateBackgroundMedia()
        {
            try
            {
                string cacheWorkaround = new string(' ', new Random().Next(1, 20));
                string defaultWallpaperImage = "Assets\\Background.png" + cacheWorkaround;
                string defaultWallpaperVideo = "Assets\\BackgroundLive.mp4" + cacheWorkaround;

                //Update video playback speed
                grid_Video_Background.SpeedRatio = (double)Convert.ToInt32(ConfigurationManager.AppSettings["BackgroundPlaySpeed"]) / 100;

                //Update background brightness
                grid_Video_Background.Opacity = (double)Convert.ToInt32(ConfigurationManager.AppSettings["BackgroundBrightness"]) / 100;

                //Set background video volume
                grid_Video_Background.Volume = 0;

                //Set background source
                if (ConfigurationManager.AppSettings["VideoBackground"] == "True")
                {
                    grid_Video_Background.Source = new Uri(defaultWallpaperVideo, UriKind.RelativeOrAbsolute);
                }
                else if (ConfigurationManager.AppSettings["DesktopBackground"] == "False")
                {
                    grid_Video_Background.Source = new Uri(defaultWallpaperImage, UriKind.RelativeOrAbsolute);
                }
                else
                {
                    string desktopWallpaper = Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "WallPaper", defaultWallpaperImage).ToString();
                    if (File.Exists(desktopWallpaper))
                    {
                        grid_Video_Background.Source = new Uri(desktopWallpaper, UriKind.RelativeOrAbsolute);
                    }
                    else
                    {
                        grid_Video_Background.Source = new Uri(defaultWallpaperImage, UriKind.RelativeOrAbsolute);
                    }
                }

                //Play background media
                grid_Video_Background.LoadedBehavior = MediaState.Manual;
                grid_Video_Background.Play();
            }
            catch
            {
                Debug.WriteLine("Failed updating the background media.");
            }
        }

        //Restart the live background video
        private void Grid_Video_Background_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                //Debug.WriteLine("Background media ended, restarting.");
                MediaElement senderMediaElement = (MediaElement)sender;
                senderMediaElement.Position = new TimeSpan(0, 0, 0, 0, 200);
            }
            catch { }
        }
    }
}