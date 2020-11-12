using ArnoldVinkCode;
using LibraryUsb;
using System;
using System.Diagnostics;
using System.Windows;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Receive rumble byte data
        public void SendXRumbleData(ControllerStatus Controller, bool ForceUpdate, bool TestLight, bool TestHeavy)
        {
            try
            {
                if (ForceUpdate || Controller.XOutputData[1] == 0x08)
                {
                    //Read the rumble strength
                    byte HeavyMotor = 0; //0-255
                    byte LightMotor = 0; //0-255
                    if (TestHeavy) { HeavyMotor = 255; } else { HeavyMotor = Controller.XOutputData[3]; }
                    if (TestLight) { LightMotor = 255; } else { LightMotor = Controller.XOutputData[4]; }

                    //Adjust the rumble strength
                    double ControllerRumbleStrength = Convert.ToDouble(Controller.Details.Profile.RumbleStrength) / 100;
                    HeavyMotor = Convert.ToByte(HeavyMotor * ControllerRumbleStrength);
                    LightMotor = Convert.ToByte(LightMotor * ControllerRumbleStrength);
                    Debug.WriteLine("Rumble Heavy motor: " + HeavyMotor + " / Light Motor: " + LightMotor);

                    //Update controller interface preview
                    if (vAppActivated && !vAppMinimized)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            if (HeavyMotor > 0 || LightMotor > 0)
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
                        Debug.WriteLine("BlueRumb DS5");
                    }
                    else if (Controller.SupportedCurrent.CodeName == "SonyDualSense5" && !Controller.Details.Wireless)
                    {
                        //Wired USB Output - DualSense 5
                        byte[] OutputReport = new byte[Controller.OutputReport.Length];
                        OutputReport[0] = 0x02;
                        OutputReport[1] = 0xFF;
                        OutputReport[2] = 0x44;
                        OutputReport[3] = LightMotor;
                        OutputReport[4] = HeavyMotor;

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
                        NativeMethods_Hid.WriteFile(Controller.HidDevice.DeviceHandle, OutputReport, (uint)OutputReport.Length, out uint bytesWritten, IntPtr.Zero);
                        Debug.WriteLine("UsbRumb DS5");
                    }
                    else if (Controller.SupportedCurrent.CodeName == "SonyDualShock4" && Controller.Details.Wireless)
                    {
                        //Bluetooth Output - DualShock 4
                        byte[] OutputReport = new byte[Controller.OutputReport.Length];
                        OutputReport[0] = 0x11;
                        OutputReport[1] = 0x80;
                        OutputReport[3] = 0xFF;
                        OutputReport[6] = LightMotor;
                        OutputReport[7] = HeavyMotor;
                        OutputReport[11] = 255; //Led On Duration
                        OutputReport[12] = 0; //Led Off Duration

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
                        NativeMethods_Hid.HidD_SetOutputReport(Controller.HidDevice.DeviceHandle, OutputReport, OutputReport.Length);
                        Debug.WriteLine("BlueRumb DS4");
                    }
                    else if (Controller.SupportedCurrent.CodeName == "SonyDualShock4" && !Controller.Details.Wireless)
                    {
                        //Wired USB Output - DualShock 4
                        byte[] OutputReport = new byte[Controller.OutputReport.Length];
                        OutputReport[0] = 0x05;
                        OutputReport[1] = 0xFF;
                        OutputReport[4] = LightMotor;
                        OutputReport[5] = HeavyMotor;
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
                        NativeMethods_Hid.WriteFile(Controller.HidDevice.DeviceHandle, OutputReport, (uint)OutputReport.Length, out uint bytesWritten, IntPtr.Zero);
                        Debug.WriteLine("UsbRumb DS4");
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

                        OutputReport[2] = (byte)(LightMotor > 0 ? 0x01 : 0x00); //On or Off
                        OutputReport[4] = HeavyMotor;

                        //Led Position 0x02, 0x04, 0x08, 0x10
                        switch (Controller.NumberId)
                        {
                            case 0: { OutputReport[9] = 0x02; break; }
                            case 1: { OutputReport[9] = 0x04; break; }
                            case 2: { OutputReport[9] = 0x08; break; }
                            case 3: { OutputReport[9] = 0x10; break; }
                        }

                        //Send data to the controller
                        int Transferred = 0;
                        Controller.WinUsbDevice.SendTransfer(0x21, 0x09, 0x0201, OutputReport, ref Transferred);
                        Debug.WriteLine("UsbRumb DS3");
                    }
                    else if (Controller.SupportedCurrent.CodeName == "SonyDualShock12")
                    {
                        //Wired USB Output - DualShock 1 and 2
                        int HeavyMotorDS2 = Convert.ToInt32(HeavyMotor) / 2;
                        byte[] OutputReport = new byte[Controller.OutputReport.Length];
                        OutputReport[0] = 0x01;
                        OutputReport[3] = Convert.ToByte(HeavyMotorDS2); //Between 0 and 127.5
                        OutputReport[4] = (byte)(LightMotor > 0 ? 0x01 : 0x00); //On or Off

                        //Send data to the controller
                        NativeMethods_Hid.WriteFile(Controller.HidDevice.DeviceHandle, OutputReport, (uint)OutputReport.Length, out uint bytesWritten, IntPtr.Zero);
                        Debug.WriteLine("UsbRumb DS1 and 2");
                    }
                }
            }
            catch { }
        }
    }
}