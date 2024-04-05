﻿using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Media.Control;
using Windows.Storage.Streams;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVAudioDevice;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.Enums;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Play or pause the media
        void MediaPlayPause()
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("MediaPlayPause", "Resuming or pausing media");
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                vFakerInputDevice.MultimediaPressRelease(KeysMediaHid.PlayPause);
            }
            catch { }
        }

        //Next media
        void MediaNext()
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("MediaNext", "Going to next media item");
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                vFakerInputDevice.MultimediaPressRelease(KeysMediaHid.Next);
            }
            catch { }
        }

        //Previous media
        void MediaPrevious()
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("MediaPrevious", "Going to previous media item");
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                vFakerInputDevice.MultimediaPressRelease(KeysMediaHid.Previous);
            }
            catch { }
        }

        //Fullscreen media
        void MediaFullscreen()
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("MediaFullscreen", "Toggling fullscreen");
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                KeysHidAction keyboardAction = new KeysHidAction()
                {
                    Modifiers = KeysModifierHid.AltLeft,
                    Key0 = KeysHid.Enter
                };
                vFakerInputDevice.KeyboardPressRelease(keyboardAction);
            }
            catch { }
        }

        //Volume Output Mute
        void VolumeOutputMute()
        {
            try
            {
                if (AudioMuteSwitch(false))
                {
                    App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Output volume muted");
                }
                else
                {
                    App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Output volume unmuted");
                }
            }
            catch { }
        }

        //Volume Input Mute
        void VolumeInputMute()
        {
            try
            {
                if (AudioMuteSwitch(true))
                {
                    App.vWindowOverlay.Notification_Show_Status("MicrophoneMute", "Input volume muted");
                }
                else
                {
                    App.vWindowOverlay.Notification_Show_Status("MicrophoneMute", "Input volume unmuted");
                }
            }
            catch { }
        }

        //Volume Down
        void VolumeDown()
        {
            try
            {
                int volumeStep = SettingLoad(vConfigurationDirectXInput, "MediaVolumeStep", typeof(int));
                int newVolume = AudioVolumeDown(volumeStep, false);
                App.vWindowOverlay.Notification_Show_Status("VolumeDown", "Decreased volume to " + newVolume);
            }
            catch { }
        }

        //Volume Up
        void VolumeUp()
        {
            try
            {
                int volumeStep = SettingLoad(vConfigurationDirectXInput, "MediaVolumeStep", typeof(int));
                int newVolume = AudioVolumeUp(volumeStep, false);
                App.vWindowOverlay.Notification_Show_Status("VolumeUp", "Increased volume to " + newVolume);
            }
            catch { }
        }

        //Request media player access
        async Task RequestMediaPlayerAccess()
        {
            try
            {
                Debug.WriteLine("Requesting SMTC access.");

                //Might cause Windows Explorer issue when looping.
                async Task<GlobalSystemMediaTransportControlsSessionManager> TaskAction()
                {
                    try
                    {
                        return await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                    }
                    catch { }
                    return null;
                }
                vSmtcSessionManager = await TaskStartTimeout(TaskAction, 2000);
                if (vSmtcSessionManager == null)
                {
                    HideMediaInformation();
                }
            }
            catch { }
        }

        //Update currently playing media information
        async Task UpdateCurrentMediaInformation()
        {
            try
            {
                //Check the current keyboard mode
                if (vKeyboardCurrentMode != KeyboardMode.Media)
                {
                    //Debug.WriteLine("Keyboard is not in media mode, no update needed.");
                    return;
                }

                //Get active media player session
                GlobalSystemMediaTransportControlsSession smtcSession = null;
                try
                {
                    smtcSession = vSmtcSessionManager.GetCurrentSession();
                    if (smtcSession == null)
                    {
                        HideMediaInformation();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed getting SMTC session: " + ex.Message);
                    HideMediaInformation();
                    await RequestMediaPlayerAccess();
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
                    AVActions.DispatcherInvoke(delegate
                    {
                        text_Information_Album.Visibility = Visibility.Collapsed;
                    });
                }
                else
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        text_Information_Album.Visibility = Visibility.Visible;
                    });
                }

                //Calculate the media progression
                double mediaProgress = 0;
                string mediaCurrent = string.Empty;
                string mediaTotal = string.Empty;
                if (mediaTimeline.Position != new TimeSpan() && mediaTimeline.EndTime != new TimeSpan())
                {
                    mediaProgress = mediaTimeline.Position.TotalSeconds * 100 / mediaTimeline.EndTime.TotalSeconds;
                    mediaCurrent = AVFunctions.SecondsToHms((int)mediaTimeline.Position.TotalSeconds, false, true);
                    mediaTotal = AVFunctions.SecondsToHms((int)mediaTimeline.EndTime.TotalSeconds, false, true);
                    AVActions.DispatcherInvoke(delegate
                    {
                        grid_Information_Progress.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        grid_Information_Progress.Visibility = Visibility.Collapsed;
                    });
                }

                //Load the media thumbnail image
                BitmapFrame thumbnailBitmap = await GetMediaThumbnail(mediaProperties.Thumbnail);

                //Update the media and volume information
                AVActions.DispatcherInvoke(delegate
                {
                    text_Information_Artist.Text = mediaArtist;
                    text_Information_Title.Text = mediaTitle;
                    text_Information_Album.Text = mediaAlbum;
                    progress_Information_Progress.Value = mediaProgress;
                    text_Information_Progress_Current.Text = mediaCurrent;
                    text_Information_Progress_Total.Text = mediaTotal;

                    if (thumbnailBitmap != null)
                    {
                        image_Information_Thumbnail.Source = thumbnailBitmap;
                    }
                    else
                    {
                        string currentImage = string.Empty;
                        if (image_Information_Thumbnail.Source != null)
                        {
                            currentImage = image_Information_Thumbnail.Source.ToString();
                        }
                        string updatedImage = "Assets/Default/Icons/Music.png";
                        if (currentImage.ToLower() != updatedImage.ToLower())
                        {
                            image_Information_Thumbnail.Source = FileToBitmapImage(new string[] { updatedImage }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        }
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

        //Update current system volume information
        private void UpdateCurrentVolumeInformation()
        {
            try
            {
                //Check if volume is currently muted
                bool currentOutputVolumeMuted = AudioMuteGetStatus(false);
                bool currentInputVolumeMuted = AudioMuteGetStatus(true);
                AVActions.DispatcherInvoke(delegate
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

                //Update volume information
                AVActions.DispatcherInvoke(delegate
                {
                    textblock_Volume_Level.Text = currentVolumeString;
                });
            }
            catch
            {
                //Debug.WriteLine("Failed updating volume.");
            }
        }

        //Hide media information
        void HideMediaInformation()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
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