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
using static DirectXInput.AppVariables;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;
using static LibraryUsb.FakerInputDevice;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Play or pause the media
        async Task MediaPlayPause()
        {
            try
            {
                await App.vWindowOverlay.Notification_Show_Status("MediaPlayPause", "Resuming or pausing media");
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.PlayPause);
            }
            catch { }
        }

        //Next media
        async Task MediaNext()
        {
            try
            {
                await App.vWindowOverlay.Notification_Show_Status("MediaNext", "Going to next media item");
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.Next);
            }
            catch { }
        }

        //Previous media
        async Task MediaPrevious()
        {
            try
            {
                await App.vWindowOverlay.Notification_Show_Status("MediaPrevious", "Going to previous media item");
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.Previous);
            }
            catch { }
        }

        //Fullscreen media
        async Task MediaFullscreen()
        {
            try
            {
                await App.vWindowOverlay.Notification_Show_Status("MediaFullscreen", "Toggling fullscreen");
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.AltLeft, KeyboardModifiers.None, KeyboardKeys.Enter, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
            }
            catch { }
        }

        //Volume Output Mute
        async Task VolumeOutputMute()
        {
            try
            {
                if (AudioMuteSwitch(false))
                {
                    await App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Output volume muted");
                }
                else
                {
                    await App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Output volume unmuted");
                }
            }
            catch { }
        }

        //Volume Input Mute
        async Task VolumeInputMute()
        {
            try
            {
                if (AudioMuteSwitch(true))
                {
                    await App.vWindowOverlay.Notification_Show_Status("MicrophoneMute", "Input volume muted");
                }
                else
                {
                    await App.vWindowOverlay.Notification_Show_Status("MicrophoneMute", "Input volume unmuted");
                }
            }
            catch { }
        }

        //Volume Down
        async Task VolumeDown()
        {
            try
            {
                int volumeStep = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "MediaVolumeStep"));
                int newVolume = AudioVolumeDown(volumeStep, false);
                await App.vWindowOverlay.Notification_Show_Status("VolumeDown", "Decreased volume to " + newVolume);
            }
            catch { }
        }

        //Volume Up
        async Task VolumeUp()
        {
            try
            {
                int volumeStep = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "MediaVolumeStep"));
                int newVolume = AudioVolumeUp(volumeStep, false);
                await App.vWindowOverlay.Notification_Show_Status("VolumeUp", "Increased volume to " + newVolume);
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
                    currentVolumeString = "Volume " + currentVolumeInt + "%";
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

                //Load the track number
                string mediaTrackNumber = string.Empty;
                int currentTrackNumber = mediaProperties.TrackNumber;
                if (currentTrackNumber > 0)
                {
                    int totalTrackNumber = mediaProperties.AlbumTrackCount;
                    if (totalTrackNumber > 0)
                    {
                        mediaTrackNumber = "(" + currentTrackNumber + "/" + totalTrackNumber + ") ";
                    }
                    else
                    {
                        mediaTrackNumber = "(" + currentTrackNumber + ") ";
                    }
                }

                //Load the media title
                string mediaTitle = mediaProperties.Title;
                if (string.IsNullOrWhiteSpace(mediaTitle))
                {
                    mediaTitle = mediaTrackNumber + "Unknown title";
                }
                else
                {
                    mediaTitle = mediaTrackNumber + mediaTitle;
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
                    button_Information_Progress.Value = mediaProgress;
                    if (thumbnailBitmap != null)
                    {
                        button_Information_Thumbnail.Source = thumbnailBitmap;
                    }
                    else
                    {
                        button_Information_Thumbnail.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Music.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    }
                    grid_MediaPlaying.Visibility = Visibility.Visible;
                    textblock_MediaNone.Visibility = Visibility.Collapsed;
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
                    grid_MediaPlaying.Visibility = Visibility.Collapsed;
                    textblock_MediaNone.Visibility = Visibility.Visible;
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