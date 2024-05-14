using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    public partial class WindowMain
    {
        public async Task Application_Exit_Prompt()
        {
            try
            {
                List<string> messageAnswers = new List<string>();
                messageAnswers.Add("Exit application");
                messageAnswers.Add("Cancel");

                string messageResult = await new AVMessageBox().Popup(vWindowMain, "Do you really want to exit DirectXInput?", "This will disconnect all your currently connected controllers.", messageAnswers);
                if (messageResult == "Exit application")
                {
                    await Application_Exit();
                }
            }
            catch { }
        }

        public async Task Application_Exit()
        {
            try
            {
                Debug.WriteLine("Exiting application.");

                //Disable application window
                vWindowMain.AppWindowDisable("Closing DirectXInput, please wait.");

                //Stop the background tasks
                await TasksBackgroundStop();

                //Disconnect all controllers
                await StopAllControllers();

                //Check if VirtualBus is connected
                if (vVirtualBusDevice != null)
                {
                    //Close VirtualBus device
                    vVirtualBusDevice.CloseDevice();
                    vVirtualBusDevice = null;
                }

                //Check if HidHide is connected
                if (vHidHideDevice != null)
                {
                    //Reset HidHide to defaults
                    vHidHideDevice.ListDeviceReset();
                    vHidHideDevice.ListApplicationReset();

                    //Disable HidHide device
                    vHidHideDevice.DeviceHideToggle(false);

                    //Close HidHide device
                    vHidHideDevice.CloseDevice();
                    vHidHideDevice = null;
                }

                //Check if FakerInput is connected
                if (vFakerInputDevice != null)
                {
                    //Close FakerInput device
                    vFakerInputDevice.CloseDevice();
                    vFakerInputDevice = null;
                }

                //Disable the socket server
                if (vArnoldVinkSockets != null)
                {
                    await vArnoldVinkSockets.SocketServerDisable();
                }

                //Hide the visible tray icon
                TrayNotifyIcon.Visible = false;

                //Close the application
                Environment.Exit(0);
            }
            catch { }
        }
    }
}