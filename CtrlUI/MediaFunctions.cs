using ArnoldVinkCode;
using System;
using System.Threading.Tasks;
using System.Windows;
using Windows.Media.Control;
using static ArnoldVinkCode.AVAudioDevice;
using static CtrlUI.AppVariables;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Update the currently playing media
        async Task UpdateCurrentMediaInformation()
        {
            try
            {
                //Check if volume is currently muted
                bool currentVolumeMuted = AudioMuteGetStatus();
                await AVActions.ActionDispatcherInvokeAsync(delegate
                {
                    if (currentVolumeMuted)
                    {
                        img_Main_VolumeMute.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        img_Main_VolumeMute.Visibility = Visibility.Collapsed;
                    }
                });

                //Check if the application window is activated
                if (!vAppActivated)
                {
                    //Debug.WriteLine("Not updating media information, not activated.");
                    return;
                }

                //Check if the media popup is opened or setting is enabled
                bool mediaUpdateSetting = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowMediaMain"));

                await AVActions.ActionDispatcherInvokeAsync(delegate
                {
                    if (!mediaUpdateSetting)
                    {
                        main_Media_Information.Visibility = Visibility.Collapsed;
                        //Debug.WriteLine("Not updating media information, disabled.");
                        return;
                    }
                    else
                    {
                        //Update the media information margin
                        double widthTopButtons = stackpanel_TopButtons.ActualWidth + 10;
                        double widthClockBattery = grid_ClockBattery.ActualWidth + grid_ClockBattery.Margin.Right + 10;
                        main_Media_Information.Margin = new Thickness(widthTopButtons, 10, widthClockBattery, 0);
                    }
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

                GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties = await smtcSession.TryGetMediaPropertiesAsync();
                GlobalSystemMediaTransportControlsSessionPlaybackInfo mediaPlayInfo = smtcSession.GetPlaybackInfo();
                //Debug.WriteLine("Media: " + mediaProperties.Title + "/" + mediaProperties.Artist + "/" + mediaProperties.AlbumTitle + "/" + mediaProperties.Subtitle + "/" + mediaProperties.PlaybackType + "/" + mediaProperties.TrackNumber + "/" + mediaProperties.AlbumTrackCount);
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

                //Update the media and volume information
                AVActions.ActionDispatcherInvoke(delegate
                {
                    main_Media_Information_Artist.Text = mediaArtist;
                    main_Media_Information_Title.Text = " " + mediaTitle;
                    main_Media_Information.Visibility = Visibility.Visible;

                    if (mediaPlayInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                    {
                        main_Media_Information_Artist.Opacity = 1;
                        main_Media_Information_Title.Opacity = 1;
                    }
                    else
                    {
                        main_Media_Information_Artist.Opacity = 0.40;
                        main_Media_Information_Title.Opacity = 0.40;
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
                });
            }
            catch { }
        }
    }
}