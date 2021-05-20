using System;
using System.Diagnostics;
using System.Windows.Media;
using static ArnoldVinkCode.AVActions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.CRC32;
using static LibraryShared.Enums;
using static LibraryShared.Settings;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Send rumble
        void LoopOutputController(ControllerStatus Controller)
        {
            try
            {
                Debug.WriteLine("Send rumble for: " + Controller.Details.DisplayName);

                //Initialize controller
                ControllerInitialize(Controller);

                //Send default output to controller
                ControllerOutput(Controller, false, false);

                //Receive output from the virtual bus
                while (!Controller.OutputControllerTask.TaskStopRequest && Controller.Connected())
                {
                    try
                    {
                        //Delay task to prevent high cpu usage
                        TaskDelayMs(1);

                        //Check if the rumble value has changed
                        bool heavyRumbleChanged = Controller.XOutputCurrentRumbleHeavy == Controller.XOutputPreviousRumbleHeavy;
                        bool lightRumbleChanged = Controller.XOutputCurrentRumbleLight == Controller.XOutputPreviousRumbleLight;
                        if (heavyRumbleChanged && lightRumbleChanged) { continue; }

                        //Update the previous rumble value
                        Controller.XOutputPreviousRumbleHeavy = Controller.XOutputCurrentRumbleHeavy;
                        Controller.XOutputPreviousRumbleLight = Controller.XOutputCurrentRumbleLight;

                        //Send received output to controller
                        ControllerOutput(Controller, false, false);
                    }
                    catch { }
                }
            }
            catch { }
        }

        //Initialize controller
        private void ControllerInitialize(ControllerStatus Controller)
        {
            try
            {
                if (Controller.SupportedCurrent.CodeName == "SonyDualShock3" || Controller.SupportedCurrent.CodeName == "SonyMoveNavigation3")
                {
                    //Wired USB Output - DualShock 3 or Move Navigation 3
                    byte[] outputReport = new byte[2];
                    outputReport[0] = 0x42;
                    outputReport[1] = 0x0C;

                    bool bytesWritten = Controller.WinUsbDevice.WriteBytesTransfer(0x21, 0x09, 0x3F4, outputReport);
                    Debug.WriteLine("Initialized USB controller: SonyDualShock3 or SonyMoveNavigation3: " + bytesWritten);
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualSense5" && Controller.Details.Wireless)
                {
                    //Bluetooth Output - DualSense 5
                    byte[] outputReport = new byte[75];
                    outputReport[0] = 0xA2;
                    outputReport[1] = 0x31;
                    outputReport[2] = 0x02;
                    outputReport[3] = 0xFF;
                    outputReport[4] = 0x08;

                    //Add CRC32 to bytes array
                    byte[] outputReportCRC32 = ByteArrayAddCRC32(outputReport);

                    //Send data to the controller
                    bool bytesWritten = Controller.HidDevice.WriteBytesFile(outputReportCRC32);
                    Debug.WriteLine("Initialized Bluetooth controller: SonyDualSense5: " + bytesWritten);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to initialize game controller: " + ex.Message);
            }
        }

        //Add CRC32 hash to bytes array
        private byte[] ByteArrayAddCRC32(byte[] outputReport)
        {
            try
            {
                //Compute CRC32 hash
                byte[] checksum = ComputeHashCRC32(outputReport, false);

                //Add CRC32 hash bytes
                byte[] outputReportCRC32 = new byte[outputReport.Length + 4];
                Array.Copy(outputReport, 1, outputReportCRC32, 0, outputReport.Length - 1);
                outputReportCRC32[74] = checksum[0];
                outputReportCRC32[75] = checksum[1];
                outputReportCRC32[76] = checksum[2];
                outputReportCRC32[77] = checksum[3];
                return outputReportCRC32;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to add CRC32 bytes to the array: " + ex.Message);
                return outputReport;
            }
        }

        //Send output to controller
        public void ControllerOutput(ControllerStatus Controller, bool testLight, bool testHeavy)
        {
            try
            {
                //Check if the controller is connected
                if (Controller == null || !Controller.Connected())
                {
                    //Debug.WriteLine("Rumble send controller is not connected.");
                    return;
                }

                //Read the rumble strength
                byte controllerRumbleHeavy = 0;
                byte controllerRumbleLight = 0;
                if (testHeavy)
                {
                    controllerRumbleHeavy = 255;
                }
                else if (testLight)
                {
                    controllerRumbleLight = 255;
                }
                else
                {
                    controllerRumbleHeavy = Controller.XOutputCurrentRumbleHeavy;
                    controllerRumbleLight = Controller.XOutputCurrentRumbleLight;
                }

                //Adjust the trigger rumble strength
                byte triggerRumbleLimit = 150;
                byte triggerRumbleMinimum = 5;
                byte triggerRumbleLeft = 0;
                byte triggerRumbleRight = 0;
                if (Controller.Details.Profile.TriggerRumbleEnabled)
                {
                    double triggerRumbleStrengthLeft = Convert.ToDouble(Controller.Details.Profile.TriggerRumbleStrengthLeft) / 100;
                    triggerRumbleLeft = Convert.ToByte(Math.Max(controllerRumbleLight, controllerRumbleHeavy) * triggerRumbleStrengthLeft);
                    if (triggerRumbleLeft > triggerRumbleLimit) { triggerRumbleLeft = triggerRumbleLimit; }
                    Debug.WriteLine("Trigger rumble left: " + triggerRumbleLeft + " / Limit: " + triggerRumbleLimit + " / Minimum: " + triggerRumbleMinimum);

                    double triggerRumbleStrengthRight = Convert.ToDouble(Controller.Details.Profile.TriggerRumbleStrengthRight) / 100;
                    triggerRumbleRight = Convert.ToByte(Math.Max(controllerRumbleLight, controllerRumbleHeavy) * triggerRumbleStrengthRight);
                    if (triggerRumbleRight > triggerRumbleLimit) { triggerRumbleRight = triggerRumbleLimit; }
                    Debug.WriteLine("Trigger rumble right: " + triggerRumbleRight + " / Limit: " + triggerRumbleLimit + " / Minimum: " + triggerRumbleMinimum);
                }

                //Adjust the controller rumble strength
                if (Controller.Details.Profile.ControllerRumbleEnabled)
                {
                    double controllerRumbleStrength = Convert.ToDouble(Controller.Details.Profile.ControllerRumbleStrength) / 100;
                    controllerRumbleHeavy = Convert.ToByte(controllerRumbleHeavy * controllerRumbleStrength);
                    controllerRumbleLight = Convert.ToByte(controllerRumbleLight * controllerRumbleStrength);
                    Debug.WriteLine("Controller rumble Heavy: " + controllerRumbleHeavy + " / Light: " + controllerRumbleLight);
                }
                else
                {
                    controllerRumbleHeavy = 0;
                    controllerRumbleLight = 0;
                }

                //Check which controller is connected
                if (Controller.SupportedCurrent.CodeName == "SonyDualSense5" && Controller.Details.Wireless)
                {
                    //Bluetooth Output - DualSense 5
                    byte[] outputReport = new byte[75];
                    outputReport[0] = 0xA2;
                    outputReport[1] = 0x31;
                    outputReport[2] = 0x02;
                    outputReport[3] = 0xFF;
                    outputReport[4] = 0xF7;

                    //Controller rumble
                    outputReport[5] = controllerRumbleLight;
                    outputReport[6] = controllerRumbleHeavy;

                    //Trigger rumble
                    if (triggerRumbleRight >= triggerRumbleMinimum)
                    {
                        outputReport[13] = 0x01; //Right trigger
                        outputReport[14] = 0x00; //Begin;
                        outputReport[15] = triggerRumbleRight; //Force
                    }
                    else
                    {
                        outputReport[13] = 0x01; //Right trigger
                        outputReport[14] = 0xFF; //Begin;
                        outputReport[15] = 0x00; //Force
                    }
                    if (triggerRumbleLeft >= triggerRumbleMinimum)
                    {
                        outputReport[24] = 0x01; //Left trigger
                        outputReport[25] = 0x00; //Begin;
                        outputReport[26] = triggerRumbleLeft; //Force
                    }
                    else
                    {
                        outputReport[24] = 0x01; //Left trigger
                        outputReport[25] = 0xFF; //Begin;
                        outputReport[26] = 0x00; //Force
                    }

                    //If volume is muted turn on mute led
                    if (vControllerMuteLed)
                    {
                        outputReport[11] = 0x01;
                    }
                    else
                    {
                        outputReport[11] = 0x00;
                    }

                    //If battery is low turn on player led
                    if (Controller.BatteryCurrent.BatteryPercentage <= Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "BatteryLowLevel")) && Controller.BatteryCurrent.BatteryStatus == BatteryStatus.Normal)
                    {
                        outputReport[46] = 0x04;
                    }
                    else
                    {
                        outputReport[46] = 0x00;
                    }

                    //Set the controller led color
                    Color controllerColor = (Color)Controller.Color;
                    double controllerLedBrightness = Convert.ToDouble(Controller.Details.Profile.LedBrightness) / 100;
                    outputReport[47] = Convert.ToByte(controllerColor.R * controllerLedBrightness);
                    outputReport[48] = Convert.ToByte(controllerColor.G * controllerLedBrightness);
                    outputReport[49] = Convert.ToByte(controllerColor.B * controllerLedBrightness);

                    //Add CRC32 to bytes array
                    byte[] outputReportCRC32 = ByteArrayAddCRC32(outputReport);

                    //Send data to the controller
                    bool bytesWritten = Controller.HidDevice.WriteBytesFile(outputReportCRC32);
                    Debug.WriteLine("BlueRumb DS5: " + bytesWritten);
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualSense5" && !Controller.Details.Wireless)
                {
                    //Wired USB Output - DualSense 5
                    byte[] outputReport = new byte[Controller.OutputReport.Length];
                    outputReport[0] = 0x02;
                    outputReport[1] = 0xFF;
                    outputReport[2] = 0xF7;

                    //Controller rumble
                    outputReport[3] = controllerRumbleLight;
                    outputReport[4] = controllerRumbleHeavy;

                    //Trigger rumble
                    if (triggerRumbleRight >= triggerRumbleMinimum)
                    {
                        outputReport[11] = 0x01; //Right trigger
                        outputReport[12] = 0x00; //Begin;
                        outputReport[13] = triggerRumbleRight; //Force
                    }
                    else
                    {
                        outputReport[11] = 0x01; //Right trigger
                        outputReport[12] = 0xFF; //Begin;
                        outputReport[13] = 0x00; //Force
                    }
                    if (triggerRumbleLeft >= triggerRumbleMinimum)
                    {
                        outputReport[22] = 0x01; //Left trigger
                        outputReport[23] = 0x00; //Begin;
                        outputReport[24] = triggerRumbleLeft; //Force
                    }
                    else
                    {
                        outputReport[22] = 0x01; //Left trigger
                        outputReport[23] = 0xFF; //Begin;
                        outputReport[24] = 0x00; //Force
                    }

                    //If volume is muted turn on mute led
                    if (vControllerMuteLed)
                    {
                        outputReport[9] = 0x01;
                    }
                    else
                    {
                        outputReport[9] = 0x00;
                    }

                    //Turn off player led
                    outputReport[44] = 0x00;

                    //Set the controller led color
                    Color controllerColor = (Color)Controller.Color;
                    double controllerLedBrightness = Convert.ToDouble(Controller.Details.Profile.LedBrightness) / 100;
                    outputReport[45] = Convert.ToByte(controllerColor.R * controllerLedBrightness);
                    outputReport[46] = Convert.ToByte(controllerColor.G * controllerLedBrightness);
                    outputReport[47] = Convert.ToByte(controllerColor.B * controllerLedBrightness);

                    //Send data to the controller
                    bool bytesWritten = Controller.HidDevice.WriteBytesFile(outputReport);
                    Debug.WriteLine("UsbRumb DS5: " + bytesWritten);
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualShock4" && Controller.Details.Wireless)
                {
                    //Bluetooth Output - DualShock 4
                    byte[] outputReport = new byte[Controller.OutputReport.Length];
                    outputReport[0] = 0x11;
                    outputReport[1] = 0x80;
                    outputReport[3] = 0xFF;
                    outputReport[6] = controllerRumbleLight;
                    outputReport[7] = controllerRumbleHeavy;

                    //If battery is low flash the led
                    if (Controller.BatteryCurrent.BatteryPercentage <= Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "BatteryLowLevel")) && Controller.BatteryCurrent.BatteryStatus == BatteryStatus.Normal)
                    {
                        outputReport[11] = 128; //Led On Duration
                        outputReport[12] = 128; //Led Off Duration
                    }
                    else
                    {
                        outputReport[11] = 255; //Led On Duration
                        outputReport[12] = 0; //Led Off Duration
                    }

                    //Set the controller led color
                    Color controllerColor = (Color)Controller.Color;
                    double controllerLedBrightness = Convert.ToDouble(Controller.Details.Profile.LedBrightness) / 100;
                    outputReport[8] = Convert.ToByte(controllerColor.R * controllerLedBrightness);
                    outputReport[9] = Convert.ToByte(controllerColor.G * controllerLedBrightness);
                    outputReport[10] = Convert.ToByte(controllerColor.B * controllerLedBrightness);

                    //Send data to the controller
                    bool bytesWritten = Controller.HidDevice.WriteBytesOutputReport(outputReport);
                    Debug.WriteLine("BlueRumb DS4: " + bytesWritten);
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualShock4" && !Controller.Details.Wireless)
                {
                    //Wired USB Output - DualShock 4
                    byte[] outputReport = new byte[Controller.OutputReport.Length];
                    outputReport[0] = 0x05;
                    outputReport[1] = 0xFF;
                    outputReport[4] = controllerRumbleLight;
                    outputReport[5] = controllerRumbleHeavy;
                    outputReport[9] = 255; //Led On Duration
                    outputReport[10] = 0; //Led Off Duration

                    //Set the controller led color
                    Color controllerColor = (Color)Controller.Color;
                    double controllerLedBrightness = Convert.ToDouble(Controller.Details.Profile.LedBrightness) / 100;
                    outputReport[6] = Convert.ToByte(controllerColor.R * controllerLedBrightness);
                    outputReport[7] = Convert.ToByte(controllerColor.G * controllerLedBrightness);
                    outputReport[8] = Convert.ToByte(controllerColor.B * controllerLedBrightness);

                    //Send data to the controller
                    bool bytesWritten = Controller.HidDevice.WriteBytesFile(outputReport);
                    Debug.WriteLine("UsbRumb DS4: " + bytesWritten);
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualShock3")
                {
                    //Wired USB Output - DualShock 3
                    byte[] outputReport = new byte[30];
                    outputReport[1] = 0xFF;
                    outputReport[2] = (byte)(controllerRumbleLight > 0 ? 0x01 : 0x00); //On or Off
                    outputReport[3] = 0xFF;
                    outputReport[4] = controllerRumbleHeavy;
                    outputReport[10] = 0xFF;
                    outputReport[11] = 0x27;
                    outputReport[12] = 0x10;
                    outputReport[14] = 0x32;
                    outputReport[15] = 0xFF;
                    outputReport[16] = 0x27;
                    outputReport[17] = 0x10;
                    outputReport[19] = 0x32;
                    outputReport[20] = 0xFF;
                    outputReport[21] = 0x27;
                    outputReport[22] = 0x10;
                    outputReport[24] = 0x32;
                    outputReport[25] = 0xFF;
                    outputReport[26] = 0x27;
                    outputReport[27] = 0x10;
                    outputReport[29] = 0x32;

                    //Led Position 0x02, 0x04, 0x08, 0x10
                    switch (Controller.NumberId)
                    {
                        case 0: { outputReport[9] = 0x02; break; }
                        case 1: { outputReport[9] = 0x04; break; }
                        case 2: { outputReport[9] = 0x08; break; }
                        case 3: { outputReport[9] = 0x10; break; }
                    }

                    //Send data to the controller
                    bool bytesWritten = Controller.WinUsbDevice.WriteBytesTransfer(0x21, 0x09, 0x0201, outputReport);
                    Debug.WriteLine("UsbRumb DS3: " + bytesWritten);
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualShock12")
                {
                    //Wired USB Output - DualShock 1 and 2
                    byte[] outputReport = new byte[Controller.OutputReport.Length];
                    outputReport[0] = 0x01;
                    outputReport[3] = (byte)(controllerRumbleHeavy / 2); //Between 0 and 127.5
                    outputReport[4] = (byte)(controllerRumbleLight > 0 ? 0x01 : 0x00); //On or Off

                    //Send data to the controller
                    bool bytesWritten = Controller.HidDevice.WriteBytesOutputReport(outputReport);
                    Debug.WriteLine("UsbRumb DS1 and 2: " + bytesWritten);
                }
                else if (testHeavy || testLight)
                {
                    //Show unsupported controller notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "Unsupported rumble controller";
                    notificationDetails.Color = Controller.Color;
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                    Debug.WriteLine("Unsupported rumble controller.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to output rumble: " + ex.Message);
            }
        }
    }
}