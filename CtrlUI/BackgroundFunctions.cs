using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static CtrlUI.AppVariables;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Unload the current background media
        void UnloadBackgroundMedia()
        {
            try
            {
                grid_Video_Background.Stop();
                grid_Video_Background.Source = null;
            }
            catch { }
        }

        //Update the application background brightness
        void UpdateBackgroundBrightness()
        {
            try
            {
                grid_Video_Background.Opacity = (double)Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "BackgroundBrightness")) / 100;
            }
            catch { }
        }

        //Update the application background play speed
        void UpdateBackgroundPlaySpeed()
        {
            try
            {
                grid_Video_Background.SpeedRatio = (double)Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "BackgroundPlaySpeed")) / 100;
            }
            catch { }
        }

        //Update the application background volume
        void UpdateBackgroundPlayVolume()
        {
            try
            {
                grid_Video_Background.Volume = (double)Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "BackgroundPlayVolume")) / 100;
            }
            catch { }
        }

        //Update the application background media
        void UpdateBackgroundMedia()
        {
            try
            {
                string cacheWorkaround = new string(' ', new Random().Next(1, 20));
                string userWallpaperImage = "Assets/User/Background.png" + cacheWorkaround;
                string userWallpaperVideo = "Assets/User/BackgroundLive.mp4" + cacheWorkaround;
                string defaultWallpaperImage = "Assets/Default/Background.png" + cacheWorkaround;
                string defaultWallpaperVideo = "Assets/Default/BackgroundLive.mp4" + cacheWorkaround;

                //Set media loaded behavior
                grid_Video_Background.LoadedBehavior = MediaState.Manual;

                //Unload the current background media
                UnloadBackgroundMedia();

                //Update the application background play speed
                UpdateBackgroundPlaySpeed();

                //Update the application background brightness
                UpdateBackgroundBrightness();

                //Update the application background volume
                UpdateBackgroundPlayVolume();

                //Set background source
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "VideoBackground")))
                {
                    if (File.Exists(userWallpaperVideo))
                    {
                        grid_Video_Background.Source = new Uri(userWallpaperVideo, UriKind.RelativeOrAbsolute);
                    }
                    else
                    {
                        grid_Video_Background.Source = new Uri(defaultWallpaperVideo, UriKind.RelativeOrAbsolute);
                    }
                }
                else if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "DesktopBackground")))
                {
                    string desktopWallpaper = Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "WallPaper", string.Empty).ToString();
                    if (File.Exists(desktopWallpaper))
                    {
                        grid_Video_Background.Source = new Uri(desktopWallpaper, UriKind.RelativeOrAbsolute);
                    }
                    else
                    {
                        if (File.Exists(userWallpaperImage))
                        {
                            grid_Video_Background.Source = new Uri(userWallpaperImage, UriKind.RelativeOrAbsolute);
                        }
                        else
                        {
                            grid_Video_Background.Source = new Uri(defaultWallpaperImage, UriKind.RelativeOrAbsolute);
                        }
                    }
                }
                else
                {
                    if (File.Exists(userWallpaperImage))
                    {
                        grid_Video_Background.Source = new Uri(userWallpaperImage, UriKind.RelativeOrAbsolute);
                    }
                    else
                    {
                        grid_Video_Background.Source = new Uri(defaultWallpaperImage, UriKind.RelativeOrAbsolute);
                    }
                }

                //Play background media
                grid_Video_Background.Play();
            }
            catch
            {
                Debug.WriteLine("Failed updating the background media.");
            }
        }

        //Restart the live background video
        private void Grid_Video_Background_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            try
            {
                Debug.WriteLine("Background media failed, restarting.");
                UpdateBackgroundMedia();
            }
            catch { }
        }

        //Restart the live background video
        private void Grid_Video_Background_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                MediaElement senderMediaElement = (MediaElement)sender;
                if (senderMediaElement.NaturalDuration != Duration.Automatic)
                {
                    //Debug.WriteLine("Background media ended, restarting: " + senderMediaElement.NaturalDuration);
                    senderMediaElement.Position = new TimeSpan(0, 0, 0, 0, 200);
                }
            }
            catch { }
        }
    }
}