using ArnoldVinkCode;
using ScreenCaptureImport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ScreenCapture
{
    public partial class AppClose
    {
        //Application close prompt
        public static async Task Application_Exit_Prompt()
        {
            try
            {
                List<string> messageAnswers = new List<string>();
                messageAnswers.Add("Close application");
                messageAnswers.Add("Cancel");

                string messageResult = await new AVMessageBox().Popup(null, "Do you really want to close Screen Capture Tool?", "You will no longer be able to take screenshots using the set shortcuts.", messageAnswers);
                if (messageResult == "Close application")
                {
                    await Application_Exit();
                }
            }
            catch { }
        }

        //Close the application
        public static async Task Application_Exit()
        {
            try
            {
                Debug.WriteLine("Exiting application.");

                //Stop active video capture
                CaptureScreen.StopCaptureVideoToFile();

                //Reset screen capture resources
                CaptureImport.CaptureReset();

                //Hide the visible tray icons
                AppTrayMenuTool.TrayNotifyIcon.Visible = false;
                AppTrayMenuNow.TrayNotifyIcon.Visible = false;

                //Stop keyboard hotkeys
                await AVInputOutputHotkey.Stop();

                //Close the application
                Environment.Exit(0);
            }
            catch { }
        }
    }
}