using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Windows;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryUsb.NativeMethods_Hid;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Receive rumble byte data
        public void SendXRumbleData(ControllerStatus Controller, bool forceUpdate, bool testLight, bool testHeavy)
        {
            try
            {
                if (!Controller.Connected)
                {
                    Debug.WriteLine("Rumble send controller is not connected.");
                    return;
                }
                if (!forceUpdate && Controller.XOutputData[1] != 0x08)
                {
                    //Debug.WriteLine("No controller rumble update needed.");
                    return;
                }

                //Read the rumble strength
                byte triggerRumbleHighest = 0; //0-255
                byte controllerRumbleHeavy = 0; //0-255
                byte controllerRumbleLight = 0; //0-255
                if (testHeavy)
                {
                    triggerRumbleHighest = 255;
                    controllerRumbleHeavy = 255;
                }
                else
                {
                    controllerRumbleHeavy = Controller.XOutputData[3];
                }
                if (testLight)
                {
                    triggerRumbleHighest = 255;
                    controllerRumbleLight = 255;
                }
                else
                {
                    controllerRumbleLight = Controller.XOutputData[4];
                }
                triggerRumbleHighest = Math.Max(controllerRumbleLight, controllerRumbleHeavy);

                //Adjust the rumble strength
                double controllerRumbleStrength = Convert.ToDouble(Controller.Details.Profile.ControllerRumbleStrength) / 100;
                controllerRumbleHeavy = Convert.ToByte(controllerRumbleHeavy * controllerRumbleStrength);
                controllerRumbleLight = Convert.ToByte(controllerRumbleLight * controllerRumbleStrength);
                double triggerRumbleStrength = Convert.ToDouble(Controller.Details.Profile.TriggerRumbleStrength) / 100;
                triggerRumbleHighest = Convert.ToByte(triggerRumbleHighest * triggerRumbleStrength);
                Debug.WriteLine("Controller rumble Heavy: " + controllerRumbleHeavy + " / Light: " + controllerRumbleLight + " | Trigger rumble: " + triggerRumbleHighest);

                //Update controller interface preview
                if (vAppActivated && !vAppMinimized)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        if (controllerRumbleHeavy > 0 || controllerRumbleLight > 0)
                        {
                            img_ControllerPreview_Rumble.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            img_ControllerPreview_Rumble.Visibility = Visibility.Collapsed;
                        }
                    });
                }

                //Check which controller is connected
                if (Controller.SupportedCurrent.CodeName == "SonyDualSense5" && Controller.Details.Wireless)
                {
                    //Bluetooth Output - DualSense 5
                    //byte[] OutputReport = new byte[Controller.OutputReport.Length];
                    //OutputReport[0] = 0x31;

                    //Send data to the controller
                    //bool bytesWritten = Controller.HidDevice.WriteBytesOutputReport(OutputReport);
                    //Debug.WriteLine("BlueRumb DS5: " + bytesWritten);
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualSense5" && !Controller.Details.Wireless)
                {
                    //Wired USB Output - DualSense 5
                    byte[] OutputReport = new byte[Controller.OutputReport.Length];
                    OutputReport[0] = 0x02;
                    OutputReport[1] = 0xFF;
                    OutputReport[2] = 0xF7;

                    //Controller rumble
                    OutputReport[3] = controllerRumbleLight;
                    OutputReport[4] = controllerRumbleHeavy;

                    //Trigger rumble
                    if (triggerRumbleHighest > 0)
                    {
                        OutputReport[11] = 0x01; //Right trigger
                        //OutputReport[12] = 0x00; //Begin;
                        OutputReport[12] = (byte)(255 - triggerRumbleHighest); //Begin;
                        OutputReport[13] = 0xFF; //Force
                        OutputReport[22] = 0x01; //Left trigger
                        //OutputReport[23] = 0x00; //Begin;
                        OutputReport[23] = (byte)(255 - triggerRumbleHighest); //Begin;
                        OutputReport[24] = 0xFF; //Force
                    }
                    else
                    {
                        OutputReport[11] = 0x00; //Right trigger
                        OutputReport[22] = 0x00; //Left trigger
                    }

                    //If volume is muted turn on mute led
                    if (vControllerMuteLed)
                    {
                        OutputReport[9] = 0x01;
                    }
                    else
                    {
                        OutputReport[9] = 0x00;
                    }

                    //Turn off player led
                    OutputReport[44] = 0x00;

                    //Set the controller led color
                    double ControllerLedBrightness = Convert.ToDouble(Controller.Details.Profile.LedBrightness) / 100;
                    if (Controller.NumberId == 0)
                    {
                        OutputReport[45] = Convert.ToByte(10 * ControllerLedBrightness); //Red
                        OutputReport[46] = Convert.ToByte(190 * ControllerLedBrightness); //Green
                        OutputReport[47] = Convert.ToByte(240 * ControllerLedBrightness); //Blue
                    }
                    else if (Controller.NumberId == 1)
                    {
                        OutputReport[45] = Convert.ToByte(240 * ControllerLedBrightness); //Red
                        OutputReport[46] = Convert.ToByte(20 * ControllerLedBrightness); //Green
                        OutputReport[47] = Convert.ToByte(10 * ControllerLedBrightness); //Blue
                    }
                    else if (Controller.NumberId == 2)
                    {
                        OutputReport[45] = Convert.ToByte(20 * ControllerLedBrightness); //Red
                        OutputReport[46] = Convert.ToByte(240 * ControllerLedBrightness); //Green
                        OutputReport[47] = Convert.ToByte(10 * ControllerLedBrightness); //Blue
                    }
                    else
                    {
                        OutputReport[45] = Convert.ToByte(240 * ControllerLedBrightness); //Red
                        OutputReport[46] = Convert.ToByte(210 * ControllerLedBrightness); //Green
                        OutputReport[47] = Convert.ToByte(5 * ControllerLedBrightness); //Blue
                    }

                    //Send data to the controller
                    bool bytesWritten = Controller.HidDevice.WriteBytesFile(OutputReport);
                    Debug.WriteLine("UsbRumb DS5: " + bytesWritten);
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualShock4" && Controller.Details.Wireless)
                {
                    //Bluetooth Output - DualShock 4
                    byte[] OutputReport = new byte[Controller.OutputReport.Length];
                    OutputReport[0] = 0x11;
                    OutputReport[1] = 0x80;
                    OutputReport[3] = 0xFF;
                    OutputReport[6] = controllerRumbleLight;
                    OutputReport[7] = controllerRumbleHeavy;

                    //If battery is low flash the led
                    if (Controller.BatteryPercentageCurrent <= 20 && Controller.BatteryPercentageCurrent >= 0)
                    {
                        OutputReport[11] = 128; //Led On Duration
                        OutputReport[12] = 128; //Led Off Duration
                    }
                    else
                    {
                        OutputReport[11] = 255; //Led On Duration
                        OutputReport[12] = 0; //Led Off Duration
                    }

                    //Set the controller led color
                    double ControllerLedBrightness = Convert.ToDouble(Controller.Details.Profile.LedBrightness) / 100;
                    if (Controller.NumberId == 0)
                    {
                        OutputReport[8] = Convert.ToByte(10 * ControllerLedBrightness); //Red
                        OutputReport[9] = Convert.ToByte(190 * ControllerLedBrightness); //Green
                        OutputReport[10] = Convert.ToByte(240 * ControllerLedBrightness); //Blue
                    }
                    else if (Controller.NumberId == 1)
                    {
                        OutputReport[8] = Convert.ToByte(240 * ControllerLedBrightness); //Red
                        OutputReport[9] = Convert.ToByte(20 * ControllerLedBrightness); //Green
                        OutputReport[10] = Convert.ToByte(10 * ControllerLedBrightness); //Blue
                    }
                    else if (Controller.NumberId == 2)
                    {
                        OutputReport[8] = Convert.ToByte(20 * ControllerLedBrightness); //Red
                        OutputReport[9] = Convert.ToByte(240 * ControllerLedBrightness); //Green
                        OutputReport[10] = Convert.ToByte(10 * ControllerLedBrightness); //Blue
                    }
                    else
                    {
                        OutputReport[8] = Convert.ToByte(240 * ControllerLedBrightness); //Red
                        OutputReport[9] = Convert.ToByte(210 * ControllerLedBrightness); //Green
                        OutputReport[10] = Convert.ToByte(5 * ControllerLedBrightness); //Blue
                    }

                    //Send data to the controller
                    bool bytesWritten = Controller.HidDevice.WriteBytesOutputReport(OutputReport);
                    Debug.WriteLine("BlueRumb DS4: " + bytesWritten);
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualShock4" && !Controller.Details.Wireless)
                {
                    //Wired USB Output - DualShock 4
                    byte[] OutputReport = new byte[Controller.OutputReport.Length];
                    OutputReport[0] = 0x05;
                    OutputReport[1] = 0xFF;
                    OutputReport[4] = controllerRumbleLight;
                    OutputReport[5] = controllerRumbleHeavy;
                    OutputReport[9] = 255; //Led On Duration
                    OutputReport[10] = 0; //Led Off Duration

                    //Set the controller led color
                    double ControllerLedBrightness = Convert.ToDouble(Controller.Details.Profile.LedBrightness) / 100;
                    if (Controller.NumberId == 0)
                    {
                        OutputReport[6] = Convert.ToByte(10 * ControllerLedBrightness); //Red
                        OutputReport[7] = Convert.ToByte(190 * ControllerLedBrightness); //Green
                        OutputReport[8] = Convert.ToByte(240 * ControllerLedBrightness); //Blue
                    }
                    else if (Controller.NumberId == 1)
                    {
                        OutputReport[6] = Convert.ToByte(240 * ControllerLedBrightness); //Red
                        OutputReport[7] = Convert.ToByte(20 * ControllerLedBrightness); //Green
                        OutputReport[8] = Convert.ToByte(10 * ControllerLedBrightness); //Blue
                    }
                    else if (Controller.NumberId == 2)
                    {
                        OutputReport[6] = Convert.ToByte(20 * ControllerLedBrightness); //Red
                        OutputReport[7] = Convert.ToByte(240 * ControllerLedBrightness); //Green
                        OutputReport[8] = Convert.ToByte(10 * ControllerLedBrightness); //Blue
                    }
                    else
                    {
                        OutputReport[6] = Convert.ToByte(240 * ControllerLedBrightness); //Red
                        OutputReport[7] = Convert.ToByte(210 * ControllerLedBrightness); //Green
                        OutputReport[8] = Convert.ToByte(5 * ControllerLedBrightness); //Blue
                    }

                    //Send data to the controller
                    bool bytesWritten = Controller.HidDevice.WriteBytesFile(OutputReport);
                    Debug.WriteLine("UsbRumb DS4: " + bytesWritten);
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualShock3")
                {
                    //Wired USB Output - DualShock 3
                    byte[] OutputReport =
                    {
                        0x00, 0xFF, 0x00, 0xFF, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00,
                        0xFF, 0x27, 0x10, 0x00, 0x32,
                        0xFF, 0x27, 0x10, 0x00, 0x32,
                        0xFF, 0x27, 0x10, 0x00, 0x32,
                        0xFF, 0x27, 0x10, 0x00, 0x32
                    };

                    OutputReport[2] = (byte)(controllerRumbleLight > 0 ? 0x01 : 0x00); //On or Off
                    OutputReport[4] = controllerRumbleHeavy;

                    //Led Position 0x02, 0x04, 0x08, 0x10
                    switch (Controller.NumberId)
                    {
                        case 0: { OutputReport[9] = 0x02; break; }
                        case 1: { OutputReport[9] = 0x04; break; }
                        case 2: { OutputReport[9] = 0x08; break; }
                        case 3: { OutputReport[9] = 0x10; break; }
                    }

                    //Send data to the controller
                    bool bytesWritten = Controller.WinUsbDevice.WriteBytesTransfer(0x21, 0x09, 0x0201, OutputReport);
                    Debug.WriteLine("UsbRumb DS3: " + bytesWritten);
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualShock12")
                {
                    //Wired USB Output - DualShock 1 and 2
                    byte[] OutputReport = new byte[Controller.OutputReport.Length];
                    OutputReport[0] = 0x01;
                    OutputReport[3] = (byte)(controllerRumbleHeavy / 2); //Between 0 and 127.5
                    OutputReport[4] = (byte)(controllerRumbleLight > 0 ? 0x01 : 0x00); //On or Off

                    //Send data to the controller
                    bool bytesWritten = Controller.HidDevice.WriteBytesFile(OutputReport);
                    Debug.WriteLine("UsbRumb DS1 and 2: " + bytesWritten);
                }
                else if (testHeavy || testLight)
                {
                    //Show unsupported controller notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "Unsupported rumble controller";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                    Debug.WriteLine("Unsupported rumble controller.");
                }
            }
            catch { }
        }
    }
}