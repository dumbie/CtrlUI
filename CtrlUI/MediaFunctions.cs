using System.Configuration;
using System.Windows;
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

                if (ConfigurationManager.AppSettings["CloseMediaScreen"] == "True")
                {
                    //Close the open popup
                    await Popup_Close_Top();
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

                if (ConfigurationManager.AppSettings["CloseMediaScreen"] == "True")
                {
                    //Close the open popup
                    await Popup_Close_Top();
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
    }
}