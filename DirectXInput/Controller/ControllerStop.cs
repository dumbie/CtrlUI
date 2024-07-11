using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Stop the desired controller
        private async Task<bool> StopController(ControllerStatus controller, string disconnectInfo, string controllerInfo)
        {
            try
            {
                //Check if the controller is connected
                if (controller == null || !controller.Connected())
                {
                    Debug.WriteLine("Controller " + controller.NumberId + " is already disconnected.");
                    return false;
                }

                //Check if the controller is disconnecting
                if (controller.Disconnecting)
                {
                    Debug.WriteLine("Controller " + controller.NumberId + " is currently disconnecting.");
                    return false;
                }

                //Update controller disconnecting status
                controller.Disconnecting = true;

                //Get controller display number
                Debug.WriteLine("Disconnecting the controller " + controller.NumberId + ": " + controller.Details.DisplayName);
                string controllerNumberDisplay = (controller.NumberId + 1).ToString();

                //Show controller disconnect notification
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Controller";
                if (string.IsNullOrWhiteSpace(disconnectInfo))
                {
                    notificationDetails.Text = "Disconnected (" + controllerNumberDisplay + ")";
                }
                else
                {
                    notificationDetails.Text = "Disconnected " + disconnectInfo + " (" + controllerNumberDisplay + ")";
                }
                notificationDetails.Color = controller.Color;
                vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Update user interface controller status
                AVActions.DispatcherInvoke(delegate
                {
                    if (string.IsNullOrWhiteSpace(controllerInfo))
                    {
                        txt_Controller_Information.Text = "Disconnected controller " + controllerNumberDisplay + ": " + controller.Details.DisplayName;
                    }
                    else
                    {
                        txt_Controller_Information.Text = controllerInfo;
                    }

                    if (controller.NumberId == 0)
                    {
                        image_Controller0.Source = vImagePreloadIconControllerDark;
                        textblock_Controller0.Text = "No controller connected";
                        textblock_Controller0CodeName.Text = string.Empty;
                        ResetControllerDebugInformation();
                    }
                    else if (controller.NumberId == 1)
                    {
                        image_Controller1.Source = vImagePreloadIconControllerDark;
                        textblock_Controller1.Text = "No controller connected";
                        textblock_Controller1CodeName.Text = string.Empty;
                        ResetControllerDebugInformation();
                    }
                    else if (controller.NumberId == 2)
                    {
                        image_Controller2.Source = vImagePreloadIconControllerDark;
                        textblock_Controller2.Text = "No controller connected";
                        textblock_Controller2CodeName.Text = string.Empty;
                        ResetControllerDebugInformation();
                    }
                    else if (controller.NumberId == 3)
                    {
                        image_Controller3.Source = vImagePreloadIconControllerDark;
                        textblock_Controller3.Text = "No controller connected";
                        textblock_Controller3CodeName.Text = string.Empty;
                        ResetControllerDebugInformation();
                    }
                });

                //Stop controller loop tasks
                await TaskStopLoop(controller.InputControllerTask, 1000);
                await TaskStopLoop(controller.OutputControllerTask, 1000);
                await TaskStopLoop(controller.OutputVirtualTask, 1000);
                await TaskStopLoop(controller.OutputGyroscopeTask, 1000);

                //Disconnect virtual controller
                if (vVirtualBusDevice != null)
                {
                    //Disconnect virtual controller
                    await vVirtualBusDevice.VirtualUnplug(controller.NumberId);
                }

                //Disconnect Hid or WinUsb device
                if (controller.WinUsbDevice != null)
                {
                    //Dispose and stop connection with controller
                    try
                    {
                        controller.WinUsbDevice.CloseDevice();
                    }
                    catch { }
                }
                else if (controller.HidDevice != null)
                {
                    //Disconnect controller from bluetooth
                    if (controller.Details.Wireless)
                    {
                        try
                        {
                            controller.HidDevice.BluetoothDisconnect();
                        }
                        catch
                        {
                            Debug.WriteLine("Failed disconnecting device from bluetooth.");
                        }
                    }

                    //Signal Windows disconnection to prevent ghost controller
                    if (controller.Details.Wireless)
                    {
                        try
                        {
                            //Fix might lock code because no timeout
                            controller.HidDevice.GetFeature(0x02);
                            controller.HidDevice.GetFeature(0x05);
                        }
                        catch
                        {
                            Debug.WriteLine("Failed signaling controller disconnection to Windows.");
                        }
                    }

                    //Dispose and stop connection with the controller
                    try
                    {
                        controller.HidDevice.CloseDevice();
                    }
                    catch
                    {
                        Debug.WriteLine("Failed disposing and stopping the controller.");
                    }
                }

                //Reset the controller status
                controller.ResetControllerStatus();

                //Check if any controller is connected
                if (!vControllerAnyConnected())
                {
                    //Close open popups
                    if (SettingLoad(vConfigurationDirectXInput, "KeyboardCloseNoController", typeof(bool)))
                    {
                        Debug.WriteLine("No controller connected closing open popups.");
                        await HideOpenPopups();
                    }
                }

                Debug.WriteLine("Successfully stopped DirectInput controller " + controller.NumberId);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed stopping the controller DirectInput " + controller.NumberId + ": " + ex.Message);
                return false;
            }
        }

        //Stop all the controllers
        async Task StopAllControllers()
        {
            try
            {
                await StopController(vController0, "all", "Disconnected all controllers.");
                await StopController(vController1, "all", "Disconnected all controllers.");
                await StopController(vController2, "all", "Disconnected all controllers.");
                await StopController(vController3, "all", "Disconnected all controllers.");
                Debug.WriteLine("Stopped all the controllers DirectInput.");
            }
            catch
            {
                Debug.WriteLine("Failed stopping all controller DirectInput.");
            }
        }
    }
}