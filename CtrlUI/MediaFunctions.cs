using System.Configuration;
using System.Windows;
using static LibraryShared.Classes;
using static LibraryShared.OutputKeyboard;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Play or pause the media
        async void Button_Media_PlayPause(object sender, RoutedEventArgs e)
        {
            try
            {
                Popup_Show_Status("Media", "Resuming or pausing media");
                KeyPressSingle((byte)KeysVirtual.MediaPlayPause, false);

                if (ConfigurationManager.AppSettings["CloseMediaScreen"] == "True")
                {
                    //Close the open popup
                    await Popup_Close_Top();
                }
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
    }
}