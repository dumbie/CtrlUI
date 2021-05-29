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
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.JsonFunctions;

namespace DirectXInput.MediaCode
{
    partial class WindowMedia
    {
        //Enable or disable trigger rumble
        void button_EnableDisableTriggerRumble_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null)
                {
                    if (activeController.Details.Profile.TriggerRumbleEnabled)
                    {
                        App.vWindowOverlay.Notification_Show_Status("Rumble", "Disabled trigger rumble");
                        activeController.Details.Profile.TriggerRumbleEnabled = false;
                    }
                    else
                    {
                        App.vWindowOverlay.Notification_Show_Status("Rumble", "Enabled trigger rumble");
                        activeController.Details.Profile.TriggerRumbleEnabled = true;
                    }
                    JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    App.vWindowMain.ControllerUpdateSettingsInterface(activeController);
                }
            }
            catch { }
        }

        //Show or hide the fps overlayer
        async void button_ShowOrHideFpsOverlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await LaunchCloseFpsOverlayer();
            }
            catch { }
        }

        //Close the media control window
        async void button_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Hide();
            }
            catch { }
        }

        //Play or pause the media
        async void button_PlayPause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("MediaPlayPause", "Resuming or pausing media");
                await KeyPressSingleAuto(KeysVirtual.MediaPlayPause);
            }
            catch { }
        }

        //Next item the media
        async void button_Next_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("MediaNext", "Going to next media item");
                await KeyPressSingleAuto(KeysVirtual.MediaNextTrack);
            }
            catch { }
        }

        //Previous item the media
        async void button_Previous_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("MediaPrevious", "Going to previous media item");
                await KeyPressSingleAuto(KeysVirtual.MediaPreviousTrack);
            }
            catch { }
        }

        //Volume Output Mute
        void button_OutputMute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Toggling output mute");
                AudioMuteSwitch(false);
            }
            catch { }
        }

        //Volume Input Mute
        void button_InputMute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("MicrophoneMute", "Toggling input mute");
                AudioMuteSwitch(true);
            }
            catch { }
        }

        //Volume Down
        async void button_VolumeDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("VolumeDown", "Decreasing volume");
                await KeyPressSingleAuto(KeysVirtual.VolumeDown);
            }
            catch { }
        }

        //Volume Up
        async void button_VolumeUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("VolumeUp", "Increasing volume");
                await KeyPressSingleAuto(KeysVirtual.VolumeUp);
            }
            catch { }
        }

        //Fullscreen media
        async void button_Fullscreen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("MediaFullscreen", "Toggling fullscreen");
                await KeyPressComboAuto(KeysVirtual.Alt, KeysVirtual.Enter);
            }
            catch { }
        }

        //Move left
        async void button_ArrowLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("ArrowLeft", "Moving left");
                await KeyPressSingleAuto(KeysVirtual.Left);
            }
            catch { }
        }

        //Move right
        async void button_ArrowRight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("ArrowRight", "Moving right");
                await KeyPressSingleAuto(KeysVirtual.Right);
            }
            catch { }
        }

        //Update the currently playing media
        async Task UpdateCurrentMediaInformation()
        {
            try
            {
                //Check if volume is currently muted
                bool currentOutputVolumeMuted = AudioMuteGetStatus(false);
                bool currentInputVolumeMuted = AudioMuteGetStatus(true);
                AVActions.ActionDispatcherInvoke(delegate
                {
                    img_Main_VolumeMute.Visibility = currentOutputVolumeMuted ? Visibility.Visible : Visibility.Collapsed;
                    img_Main_MicrophoneMute.Visibility = currentInputVolumeMuted ? Visibility.Visible : Visibility.Collapsed;
                });

                //Get the current audio volume and mute status
                string currentVolumeString = string.Empty;
                int currentVolumeInt = AudioVolumeGet(false);
                if (currentVolumeInt >= 0)
                {
                    currentVolumeString = "System volume " + currentVolumeInt + "%";
                    if (currentOutputVolumeMuted)
                    {
                        currentVolumeString += " (Muted)";
                    }
                }

                //Update the media and volume information
                AVActions.ActionDispatcherInvoke(delegate
                {
                    textblock_Volume_Level.Text = currentVolumeString;
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
                        button_Information_Album.Visibility = Visibility.Collapsed;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        button_Information_Album.Visibility = Visibility.Visible;
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
                        button_Information_Track.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        button_Information_Track.Visibility = Visibility.Collapsed;
                    });
                }

                //Calculate the media progression
                double mediaProgress = 0;
                if (mediaTimeline.Position != new TimeSpan() && mediaTimeline.EndTime != new TimeSpan())
                {
                    mediaProgress = mediaTimeline.Position.TotalSeconds * 100 / mediaTimeline.EndTime.TotalSeconds;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        button_Information_Progress.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        button_Information_Progress.Visibility = Visibility.Collapsed;
                    });
                }

                //Load the media thumbnail image
                BitmapFrame thumbnailBitmap = await GetMediaThumbnail(mediaProperties.Thumbnail);

                //Update the media and volume information
                AVActions.ActionDispatcherInvoke(delegate
                {
                    button_Information_Artist.Text = mediaArtist;
                    button_Information_Title.Text = mediaTitle;
                    button_Information_Album.Text = mediaAlbum;
                    button_Information_Track.Text = mediaTrack;
                    button_Information_Progress.Value = mediaProgress;
                    if (thumbnailBitmap != null)
                    {
                        button_Information_Thumbnail.Source = thumbnailBitmap;
                    }
                    else
                    {
                        button_Information_Thumbnail.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Music.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    }
                    button_Information.Visibility = Visibility.Visible;

                    if (mediaPlayInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                    {
                        string currentImage = button_PlayPause_Image.Source.ToString();
                        string updatedImage = "Assets/Default/Icons/Pause.png";
                        if (currentImage.ToLower() != updatedImage.ToLower())
                        {
                            button_PlayPause_Image.Source = FileToBitmapImage(new string[] { updatedImage }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        }
                    }
                    else
                    {
                        string currentImage = button_PlayPause_Image.Source.ToString();
                        string updatedImage = "Assets/Default/Icons/Play.png";
                        if (currentImage.ToLower() != updatedImage.ToLower())
                        {
                            button_PlayPause_Image.Source = FileToBitmapImage(new string[] { updatedImage }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        }
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
                    button_Information.Visibility = Visibility.Collapsed;
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