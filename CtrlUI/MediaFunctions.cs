using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Media.Control;
using Windows.Storage.Streams;
using static LibraryShared.Classes;
using static LibraryShared.OutputKeyboard;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Play or pause the media
        void Button_Media_PlayPause(object sender, RoutedEventArgs e)
        {
            try
            {
                Popup_Show_Status("Media", "Resuming or pausing media");
                KeyPressSingle((byte)KeysVirtual.MediaPlayPause, false);
            }
            catch { }
        }

        //Next item the media
        async void Button_Media_NextItem(object sender, RoutedEventArgs e)
        {
            try
            {
                Popup_Show_Status("Next", "Going to next media item");
                KeyPressSingle((byte)KeysVirtual.MediaNextTrack, false);

                //Close all open popups
                if (ConfigurationManager.AppSettings["CloseMediaScreen"] == "True")
                {
                    await Popup_Close_All();
                }
            }
            catch { }
        }

        //Previous item the media
        async void Button_Media_PreviousItem(object sender, RoutedEventArgs e)
        {
            try
            {
                Popup_Show_Status("Previous", "Going to previous media item");
                KeyPressSingle((byte)KeysVirtual.MediaPrevTrack, false);

                //Close all open popups
                if (ConfigurationManager.AppSettings["CloseMediaScreen"] == "True")
                {
                    await Popup_Close_All();
                }
            }
            catch { }
        }

        //Volume Mute
        void Button_Media_VolumeMute(object sender, RoutedEventArgs e)
        {
            try
            {
                Popup_Show_Status("VolumeMute", "Muting volume");
                KeyPressSingle((byte)KeysVirtual.VolumeMute, false);
            }
            catch { }
        }

        //Volume Down
        void Button_Media_VolumeDown(object sender, RoutedEventArgs e)
        {
            try
            {
                Popup_Show_Status("VolumeDown", "Decreasing volume");
                KeyPressSingle((byte)KeysVirtual.VolumeDown, false);
            }
            catch { }
        }

        //Volume Up
        void Button_Media_VolumeUp(object sender, RoutedEventArgs e)
        {
            try
            {
                Popup_Show_Status("VolumeUp", "Increasing volume");
                KeyPressSingle((byte)KeysVirtual.VolumeUp, false);
            }
            catch { }
        }

        //Update the currently playing media
        async Task UpdateCurrentMediaInformation()
        {
            try
            {
                //Check if the media popup is opened
                bool MediaPopupOpen = false;
                await AVActions.ActionDispatcherInvokeAsync(delegate { MediaPopupOpen = grid_Popup_Media.Visibility == Visibility.Visible; });
                if (!MediaPopupOpen)
                {
                    return;
                }

                GlobalSystemMediaTransportControlsSessionManager smtcSessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                GlobalSystemMediaTransportControlsSession smtcSession = smtcSessionManager.GetCurrentSession();
                GlobalSystemMediaTransportControlsSessionTimelineProperties mediaTimeline = smtcSession.GetTimelineProperties();
                GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties = await smtcSession.TryGetMediaPropertiesAsync();

                //Debug.WriteLine("Media: " + mediaProperties.Title + "/" + mediaProperties.Artist + "/" + mediaProperties.Subtitle + "/" + mediaProperties.PlaybackType + "/" + mediaProperties.TrackNumber);
                //Debug.WriteLine("Time: " + mediaTimeline.Position + "/" + mediaTimeline.StartTime + "/" + mediaTimeline.EndTime);

                //Load the media artist
                string mediaArtist = mediaProperties.Artist;
                if (string.IsNullOrWhiteSpace(mediaArtist))
                {
                    mediaArtist = mediaProperties.Subtitle;
                    if (string.IsNullOrWhiteSpace(mediaArtist))
                    {
                        mediaArtist = "Unknown artist";
                    }
                }

                //Load the media title
                string mediaTitle = mediaProperties.Title;
                if (string.IsNullOrWhiteSpace(mediaTitle))
                {
                    mediaTitle = "Unknown title";
                }

                //Calculate the media progression
                double mediaProgress = 0;
                if (mediaTimeline.EndTime != new TimeSpan())
                {
                    mediaProgress = mediaTimeline.Position.TotalSeconds * 100 / mediaTimeline.EndTime.TotalSeconds;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_Media_Information_Progress.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_Media_Information_Progress.Visibility = Visibility.Collapsed;
                    });
                }

                //Load the media thumbnail image
                BitmapFrame thumbnailBitmap = await GetMediaThumbnail(mediaProperties.Thumbnail);

                //Update the media information
                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_Popup_Media_Information_Artist.Text = mediaArtist;
                    grid_Popup_Media_Information_Title.Text = mediaTitle;
                    grid_Popup_Media_Information_Progress.Value = mediaProgress;
                    grid_Popup_Media_Information_Thumbnail.Source = thumbnailBitmap;
                    grid_Popup_Media_Information.Visibility = Visibility.Visible;
                });
            }
            catch
            {
                //Debug.WriteLine("Failed updating playing media.");
                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_Popup_Media_Information.Visibility = Visibility.Collapsed;
                });
            }
        }

        //Update media thumbnail
        async Task<BitmapFrame> GetMediaThumbnail(IRandomAccessStreamReference mediaThumbnail)
        {
            try
            {
                using (IRandomAccessStreamWithContentType streamReference = await mediaThumbnail.OpenReadAsync())
                {
                    using (Stream stream = streamReference.AsStream())
                    {
                        return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Failed loading media thumbnail.");
                return null;
            }
        }
    }
}