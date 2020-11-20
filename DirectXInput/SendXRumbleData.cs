using ArnoldVinkCode;
using LibraryShared;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

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
                    byte[] OutputReportData = new byte[75];
                    OutputReportData[0] = 0xa2;
                    OutputReportData[1] = 0x31;
                    OutputReportData[2] = 0x02;
                    OutputReportData[3] = 0x03;

                    //Controller rumble
                    OutputReportData[5] = controllerRumbleLight;
                    OutputReportData[6] = controllerRumbleHeavy;

                    //Calculate CRC32
                    byte[] checksum;
                    using (Crc32 crc32Hasher = new Crc32())
                    {
                        checksum = crc32Hasher.ComputeHash(OutputReportData, 0, OutputReportData.Length).Reverse().ToArray();
                    }

                    byte[] OutputReportCRC32 = new byte[78];
                    OutputReportCRC32[0] = 0x31;
                    OutputReportCRC32[1] = 0x02;
                    OutputReportCRC32[2] = 0x03;

                    //Controller rumble
                    OutputReportCRC32[4] = controllerRumbleLight;
                    OutputReportCRC32[5] = controllerRumbleHeavy;

                    OutputReportCRC32[74] = checksum[0];
                    OutputReportCRC32[75] = checksum[1];
                    OutputReportCRC32[76] = checksum[2];
                    OutputReportCRC32[77] = checksum[3];

                    //Send data to the controller
                    bool bytesWritten = Controller.HidDevice.WriteBytesFile(OutputReportCRC32);
                    Debug.WriteLine("BlueRumb DS5: " + bytesWritten);
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
                    if (triggerRumbleHighest > 10)
                    {
                        byte triggerRumbleBegin = (byte)(255 - triggerRumbleHighest);
                        if (triggerRumbleBegin > 200) { triggerRumbleBegin = 200; }
                        OutputReport[11] = 0x01; //Right trigger
                        OutputReport[12] = triggerRumbleBegin; //Begin;
                        OutputReport[13] = 0xFF; //Force
                        OutputReport[22] = 0x01; //Left trigger
                        OutputReport[23] = triggerRumbleBegin; //Begin;
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
                    byte[] OutputReport = new byte[21];
                    OutputReport[1] = 0xFF;
                    OutputReport[2] = (byte)(controllerRumbleLight > 0 ? 0x01 : 0x00); //On or Off
                    OutputReport[3] = 0xFF;
                    OutputReport[4] = controllerRumbleHeavy;
                    OutputReport[10] = 0xFF;
                    OutputReport[11] = 0x27;
                    OutputReport[12] = 0x10;
                    OutputReport[14] = 0x32;
                    OutputReport[15] = 0xFF;
                    OutputReport[16] = 0x27;
                    OutputReport[17] = 0x10;
                    OutputReport[19] = 0x32;
                    OutputReport[20] = 0xFF;

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