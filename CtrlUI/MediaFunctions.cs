using ArnoldVinkCode;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Media.Control;
using Windows.Storage.Streams;
using static ArnoldVinkCode.AVAudioDevice;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static CtrlUI.AppVariables;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Play or pause the media
        async void Button_Media_PlayPause(object sender, RoutedEventArgs e)
        {
            try
            {
                await Notification_Send_Status("Media", "Resuming or pausing media");
                await KeyPressSingleAuto(KeysVirtual.MediaPlayPause);
            }
            catch { }
        }

        //Next item the media
        async void Button_Media_NextItem(object sender, RoutedEventArgs e)
        {
            try
            {
                await Notification_Send_Status("Next", "Going to next media item");
                await KeyPressSingleAuto(KeysVirtual.MediaNextTrack);

                //Close all open popups
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "CloseMediaScreen")))
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
                await Notification_Send_Status("Previous", "Going to previous media item");
                await KeyPressSingleAuto(KeysVirtual.MediaPreviousTrack);

                //Close all open popups
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "CloseMediaScreen")))
                {
                    await Popup_Close_All();
                }
            }
            catch { }
        }

        //Volume Mute
        async void Button_Media_VolumeMute(object sender, RoutedEventArgs e)
        {
            try
            {
                await Notification_Send_Status("VolumeMute", "Switching mute");
                await KeyPressSingleAuto(KeysVirtual.VolumeMute);
            }
            catch { }
        }

        //Volume Down
        async void Button_Media_VolumeDown(object sender, RoutedEventArgs e)
        {
            try
            {
                await Notification_Send_Status("VolumeDown", "Decreasing volume");
                await KeyPressSingleAuto(KeysVirtual.VolumeDown);
            }
            catch { }
        }

        //Volume Up
        async void Button_Media_VolumeUp(object sender, RoutedEventArgs e)
        {
            try
            {
                await Notification_Send_Status("VolumeUp", "Increasing volume");
                await KeyPressSingleAuto(KeysVirtual.VolumeUp);
            }
            catch { }
        }

        //Update the currently playing media
        async Task UpdateCurrentMediaInformation()
        {
            try
            {
                //Check if the application window is activated
                if (!vAppActivated)
                {
                    //Debug.WriteLine("Not updating media information, not activated.");
                    return;
                }

                //Check if the media popup is opened or setting is enabled
                bool mediaUpdatePopup = false;
                bool mediaUpdateSetting = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowMediaMain"));

                await AVActions.ActionDispatcherInvokeAsync(delegate
                {
                    mediaUpdatePopup = grid_Popup_Media.Visibility == Visibility.Visible;
                    if (!mediaUpdateSetting)
                    {
                        main_Media_Information.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        //Update the media information margin
                        double widthTopButtons = stackpanel_TopButtons.ActualWidth + 10;
                        double widthClockBattery = grid_ClockBattery.ActualWidth + grid_ClockBattery.Margin.Right + 10;
                        main_Media_Information.Margin = new Thickness(widthTopButtons, 10, widthClockBattery, 0);
                    }
                });

                if (!mediaUpdatePopup && !mediaUpdateSetting)
                {
                    //Debug.WriteLine("Not updating media information, disabled.");
                    return;
                }

                //Get the current audio volume and mute status
                string currentVolumeString = string.Empty;
                int currentVolumeInt = AudioVolumeGet();
                bool currentVolumeMuted = AudioMuteGetStatus();
                if (currentVolumeInt >= 0)
                {
                    currentVolumeString = "Volume " + currentVolumeInt + "%";
                    if (currentVolumeMuted)
                    {
                        currentVolumeString += " (Muted)";
                    }
                }

                //Update the media and volume information
                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_Popup_Media_Volume_Level.Text = currentVolumeString;
                });

                //Get the media session manager
                GlobalSystemMediaTransportControlsSessionManager smtcSessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                if (smtcSessionManager == null)
                {
                    HideMediaInformation();
                    return;
                }

                //Get the current media session
                GlobalSystemMediaTransportControlsSession smtcSession = smtcSessionManager.GetCurrentSession();
                if (smtcSession == null)
                {
                    HideMediaInformation();
                    return;
                }

                GlobalSystemMediaTransportControlsSessionTimelineProperties mediaTimeline = smtcSession.GetTimelineProperties();
                GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties = await smtcSession.TryGetMediaPropertiesAsync();
                GlobalSystemMediaTransportControlsSessionPlaybackInfo mediaPlayInfo = smtcSession.GetPlaybackInfo();

                //Debug.WriteLine("Media: " + mediaProperties.Title + "/" + mediaProperties.Artist + "/" + mediaProperties.AlbumTitle + "/" + mediaProperties.Subtitle + "/" + mediaProperties.PlaybackType + "/" + mediaProperties.TrackNumber + "/" + mediaProperties.AlbumTrackCount);
                //Debug.WriteLine("Time: " + mediaTimeline.Position + "/" + mediaTimeline.StartTime + "/" + mediaTimeline.EndTime);
                //Debug.WriteLine("Play: " + mediaPlayInfo.PlaybackStatus + "/" + mediaPlayInfo.PlaybackType);

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

                //Load the media album title
                string mediaAlbum = mediaProperties.AlbumTitle;
                if (string.IsNullOrWhiteSpace(mediaAlbum))
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_Media_Information_Album.Visibility = Visibility.Collapsed;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_Media_Information_Album.Visibility = Visibility.Visible;
                    });
                }

                //Load the track number
                string mediaTrack = "Track ";
                int currentTrackNumber = mediaProperties.TrackNumber;
                if (currentTrackNumber > 0)
                {
                    int totalTrackNumber = mediaProperties.AlbumTrackCount;
                    if (totalTrackNumber > 0)
                    {
                        mediaTrack += currentTrackNumber + "/" + totalTrackNumber;
                    }
                    else
                    {
                        mediaTrack += currentTrackNumber.ToString();
                    }
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_Media_Information_Track.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_Media_Information_Track.Visibility = Visibility.Collapsed;
                    });
                }

                //Calculate the media progression
                double mediaProgress = 0;
                if (mediaTimeline.Position != new TimeSpan() && mediaTimeline.EndTime != new TimeSpan())
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

                //Update the media and volume information
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (mediaUpdateSetting)
                    {
                        main_Media_Information_Artist.Text = mediaArtist;
                        main_Media_Information_Title.Text = " " + mediaTitle;
                        if (currentVolumeInt == 0 || currentVolumeMuted)
                        {
                            main_Media_Information_Volume.Text = " (Muted)";
                        }
                        else
                        {
                            main_Media_Information_Volume.Text = string.Empty;
                        }
                        main_Media_Information.Visibility = Visibility.Visible;
                    }

                    grid_Popup_Media_Information_Artist.Text = mediaArtist;
                    grid_Popup_Media_Information_Title.Text = mediaTitle;
                    grid_Popup_Media_Information_Album.Text = mediaAlbum;
                    grid_Popup_Media_Information_Track.Text = mediaTrack;
                    grid_Popup_Media_Information_Progress.Value = mediaProgress;
                    grid_Popup_Media_Information_Thumbnail.Source = thumbnailBitmap;
                    grid_Popup_Media_Information.Visibility = Visibility.Visible;

                    if (mediaPlayInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                    {
                        string currentImage = grid_Popup_Media_PlayPause_Image.Source.ToString();
                        string updatedImage = "Assets/Icons/Pause.png";
                        if (currentImage.ToLower() != updatedImage.ToLower())
                        {
                            grid_Popup_Media_PlayPause_Image.Source = FileToBitmapImage(new string[] { updatedImage }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        }

                        main_Media_Information_Artist.Opacity = 1;
                        main_Media_Information_Title.Opacity = 1;
                        main_Media_Information_Volume.Opacity = 1;
                    }
                    else
                    {
                        string currentImage = grid_Popup_Media_PlayPause_Image.Source.ToString();
                        string updatedImage = "Assets/Icons/Play.png";
                        if (currentImage.ToLower() != updatedImage.ToLower())
                        {
                            grid_Popup_Media_PlayPause_Image.Source = FileToBitmapImage(new string[] { updatedImage }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        }

                        main_Media_Information_Artist.Opacity = 0.40;
                        main_Media_Information_Title.Opacity = 0.40;
                        main_Media_Information_Volume.Opacity = 0.40;
                    }
                });
            }
            catch
            {
                //Debug.WriteLine("Failed updating playing media.");
                HideMediaInformation();
            }
        }

        //Hide media information
        void HideMediaInformation()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    main_Media_Information.Visibility = Visibility.Collapsed;
                    grid_Popup_Media_Information.Visibility = Visibility.Collapsed;
                });
            }
            catch { }
        }

        //Update media thumbnail
        async Task<BitmapFrame> GetMediaThumbnail(IRandomAccessStreamReference mediaThumbnail)
        {
            try
            {
                if (mediaThumbnail == null) { return null; }
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
                //Debug.WriteLine("Failed loading media thumbnail.");
                return null;
            }
        }
    }
}