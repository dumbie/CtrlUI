using System.Diagnostics;
using System.Windows;
using static ArnoldVinkCode.AVShellInfo;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Update drivers buttons
        async void btn_Settings_InstallDrivers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Message_UpdateDrivers();
            }
            catch { }
        }

        //Open Xbox game bar settings
        void Btn_Settings_OpenXboxGameBar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("ms-settings:gaming-gamebar");
            }
            catch { }
        }

        //Open Xbox capture settings
        void Btn_Settings_OpenXboxCapture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("ms-settings:gaming-gamedvr");
            }
            catch { }
        }

        //Open Xbox capture folder
        void Btn_Settings_OpenCaptureFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string capturesPath = ShellPath_KnownFolder(KnownFolder.AppCaptures);
                Process.Start(capturesPath);
            }
            catch { }
        }
    }
}