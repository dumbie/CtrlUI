using ArnoldVinkCode;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Update controller debug information
        void UpdateControllerDebugInformation(ControllerStatus controller)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Set basic information
                    textblock_LiveDebugInformation.Text = GenerateControllerDebugString(false);

                    //Check controller header
                    int controllerOffset = 0;
                    if (controller.Details.Wireless)
                    {
                        controllerOffset = controller.SupportedCurrent.OffsetWireless;
                    }
                    else
                    {
                        controllerOffset = controller.SupportedCurrent.OffsetWired;
                    }

                    //Set controller input
                    listbox_LiveDebugInput.Visibility = Visibility.Visible;
                    byte[] controllerRawInput = controller.ControllerDataInput;
                    if (controllerRawInput.Length > 180) { controllerRawInput = controllerRawInput.Take(180).ToArray(); }
                    for (int packetId = 0; packetId < controllerRawInput.Length; packetId++)
                    {
                        ProfileShared profileShared = new ProfileShared();
                        if (packetId < controllerOffset)
                        {
                            profileShared.String1 = "H";
                        }
                        else
                        {
                            profileShared.String1 = (packetId - controllerOffset).ToString();
                        }

                        if ((bool)cb_DebugShowHex.IsChecked)
                        {
                            profileShared.String2 = controllerRawInput[packetId].ToString("X2");
                        }
                        else
                        {
                            profileShared.String2 = controllerRawInput[packetId].ToString();
                        }
                        vControllerDebugInput[packetId] = profileShared;
                    }

                    //Gyroscope
                    stackpanel_DebugGyro.Visibility = Visibility.Visible;
                    slider_DebugGyroPitch.Value = controller.InputCurrent.GyroPitch;
                    slider_DebugGyroRoll.Value = controller.InputCurrent.GyroRoll;
                    slider_DebugGyroYaw.Value = controller.InputCurrent.GyroYaw;

                    //Accelerometer
                    stackpanel_DebugAccel.Visibility = Visibility.Visible;
                    slider_DebugAccelX.Value = controller.InputCurrent.AccelX;
                    slider_DebugAccelY.Value = controller.InputCurrent.AccelY;
                    slider_DebugAccelZ.Value = controller.InputCurrent.AccelZ;
                });
            }
            catch { }
        }

        //Reset controller debug information
        void ResetControllerDebugInformation()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Set basic information
                    textblock_LiveDebugInformation.Text = "Connect a controller to show debug information.";

                    //Hide controller input
                    listbox_LiveDebugInput.Visibility = Visibility.Collapsed;

                    //Hide gyro and accel
                    stackpanel_DebugGyro.Visibility = Visibility.Collapsed;
                    stackpanel_DebugAccel.Visibility = Visibility.Collapsed;
                });
            }
            catch { }
        }

        //Copy controller debug information
        void Btn_CopyDebugInformation_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null && activeController.ControllerDataInput != null)
                {
                    Clipboard.SetText(GenerateControllerDebugString(true));

                    Debug.WriteLine("Controller debug information copied to clipboard.");
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Paste";
                    notificationDetails.Text = "Debug information copied";
                    vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
                else
                {
                    Debug.WriteLine("Controller debug information is not available.");
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "No controller connected";
                    vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
            }
            catch { }
        }

        //Generate controller debug information string
        string GenerateControllerDebugString(bool includeRawData)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null && activeController.ControllerDataInput != null && activeController.ControllerDataOutput != null)
                {
                    //Controller input details
                    string rawPackets = "(Out" + activeController.ControllerDataOutput.Length + "/In" + activeController.ControllerDataInput.Length + ")";
                    if (activeController.Details.Wireless)
                    {
                        rawPackets += "(OffHdWs" + activeController.SupportedCurrent.OffsetWireless + ")";
                    }
                    else
                    {
                        rawPackets += "(OffHdWd" + activeController.SupportedCurrent.OffsetWired + ")";
                    }
                    rawPackets += "(ProductId" + activeController.Details.Profile.ProductID + "/VendorId" + activeController.Details.Profile.VendorID + ")";

                    //Controller input raw
                    if (includeRawData)
                    {
                        int controllerOffset = 0;
                        if (activeController.Details.Wireless)
                        {
                            controllerOffset = activeController.SupportedCurrent.OffsetWireless;
                        }
                        else
                        {
                            controllerOffset = activeController.SupportedCurrent.OffsetWired;
                        }

                        rawPackets += "\n";
                        for (int packetId = 0; packetId < activeController.ControllerDataInput.Length; packetId++)
                        {
                            string packetString = string.Empty;
                            if ((bool)cb_DebugShowHex.IsChecked)
                            {
                                packetString = activeController.ControllerDataInput[packetId].ToString("X2");
                            }
                            else
                            {
                                packetString = activeController.ControllerDataInput[packetId].ToString();
                            }

                            if (packetId < controllerOffset)
                            {
                                rawPackets = rawPackets + "H/" + packetString + " ";
                            }
                            else
                            {
                                rawPackets = rawPackets + (packetId - controllerOffset) + "/" + packetString + " ";
                            }
                        }
                    }

                    return rawPackets;
                }
                else
                {
                    return "Connect a controller to show debug information.";
                }
            }
            catch { }
            return "Failed to generate debug information.";
        }
    }
}