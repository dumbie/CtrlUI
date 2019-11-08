using ArnoldVinkCode;
using LibraryUsb;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Start Monitoring Direct Input Controllers
        async Task<bool> StartControllerDirectInput(ControllerStatus Controller)
        {
            try
            {
                if (Controller.Connected != null)
                {
                    Debug.WriteLine("Initializing direct input for: " + Controller.Connected.DisplayName);

                    //Open the selected controller
                    if (!await OpenController(Controller))
                    {
                        Debug.WriteLine("Failed to initialize direct input for: " + Controller.Connected.DisplayName);
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            txt_Controller_Information.Text = "The controller is no longer connected or supported.";
                        });

                        await StopController(Controller, false);
                        return false;
                    }

                    //Open the Xbox 360 bus driver
                    if (!await OpenX360Bus(Controller))
                    {
                        Debug.WriteLine("Failed to open x360 bus driver for: " + Controller.Connected.DisplayName);
                        return false;
                    }

                    //Set controller interface information
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Controller_Information.Text = "Connected controller " + (Controller.NumberId + 1) + ": " + Controller.Connected.DisplayName;
                    });

                    //Update the controller interface settings
                    UpdateControllerSettingsInterface(Controller);

                    //Update the last controller active time
                    Controller.LastActive = Environment.TickCount;

                    //Start Translating DirectInput Controller Threads
                    if (Controller.Connected.Type == "Win")
                    {
                        Controller.InputTaskToken = new CancellationTokenSource();
                        async void TaskAction()
                        {
                            try
                            {
                                await ReceiveWinInputData(Controller);
                            }
                            catch { }
                        }
                        Controller.InputTask = AVActions.TaskStart(TaskAction, Controller.InputTaskToken);
                    }
                    else
                    {
                        Controller.InputTaskToken = new CancellationTokenSource();
                        async void TaskAction()
                        {
                            try
                            {
                                await ReceiveHidInputData(Controller);
                            }
                            catch { }
                        }
                        Controller.InputTask = AVActions.TaskStart(TaskAction, Controller.InputTaskToken);
                    }

                    return true;
                }
            }
            catch { }
            return false;
        }

        //Update the controller interface settings
        void UpdateControllerSettingsInterface(ControllerStatus Controller)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    cb_ControllerFakeGuideButton.IsChecked = Controller.Connected.Profile.FakeGuideButton;
                    cb_ControllerUseButtonTriggers.IsChecked = Controller.Connected.Profile.UseButtonTriggers;
                    cb_ControllerDPadFourWayMovement.IsChecked = Controller.Connected.Profile.DPadFourWayMovement;
                    cb_ControllerThumbFlipMovement.IsChecked = Controller.Connected.Profile.ThumbFlipMovement;
                    cb_ControllerThumbFlipAxesLeft.IsChecked = Controller.Connected.Profile.ThumbFlipAxesLeft;
                    cb_ControllerThumbFlipAxesRight.IsChecked = Controller.Connected.Profile.ThumbFlipAxesRight;
                    cb_ControllerThumbReverseAxesLeft.IsChecked = Controller.Connected.Profile.ThumbReverseAxesLeft;
                    cb_ControllerThumbReverseAxesRight.IsChecked = Controller.Connected.Profile.ThumbReverseAxesRight;
                    textblock_ControllerRumbleStrength.Text = "Rumble strength: " + Convert.ToInt32(Controller.Connected.Profile.RumbleStrength) + "%";
                    slider_ControllerRumbleStrength.Value = Controller.Connected.Profile.RumbleStrength;
                    textblock_ControllerLedBrightness.Text = "Led brightness: " + Convert.ToInt32(Controller.Connected.Profile.LedBrightness) + "%";
                    slider_ControllerLedBrightness.Value = Controller.Connected.Profile.LedBrightness;
                });
            }
            catch { }
        }

        //Stop the desired controller
        async Task<bool> StopController(ControllerStatus Controller, bool RemoveAll360Bus)
        {
            try
            {
                //Update user interface controller status
                if (Controller.Connected != null)
                {
                    Debug.WriteLine("Disconnecting the controller " + Controller.NumberId + ": " + Controller.Connected.DisplayName);
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Controller_Information.Text = "Disconnected controller " + (Controller.NumberId + 1) + ": " + Controller.Connected.DisplayName;
                        if (Controller.NumberId == 0) { textblock_Controller0.Text = "No controller connected"; }
                        else if (Controller.NumberId == 1) { textblock_Controller1.Text = "No controller connected"; }
                        else if (Controller.NumberId == 2) { textblock_Controller2.Text = "No controller connected"; }
                        else if (Controller.NumberId == 3) { textblock_Controller3.Text = "No controller connected"; }
                    });
                }

                //Stop the controller task thread
                Controller.InputTaskToken = null;
                Controller.InputTask = null;
                await Task.Delay(500);

                //Reset the controller status
                Controller.ResetControllerStatus();

                //Disconnect Emulated Controllers
                if (Controller.X360Device != null)
                {
                    //Send empty controller information
                    PrepareXInputData(Controller, true);
                    Controller.X360Device.Report(Controller.XInputData, Controller.XOutputData);
                    await Task.Delay(500);

                    //Stop and disconnect the x360 device
                    if (RemoveAll360Bus) { Controller.X360Device.UnplugAll(); }
                    Controller.X360Device.Unplug(Controller.NumberId);
                    Controller.X360Device.Dispose();
                    Controller.X360Device = null;

                    //Reset the controller connect status
                    Controller.NumberId = -1;

                    //Reset the input and output report
                    Controller.XInputData = new byte[28];
                    Controller.XOutputData = new byte[8];
                }

                //Stop and Close the Win Usb Device
                if (Controller.WinUsbDevice != null)
                {
                    try
                    {
                        //Dispose and stop connection with the controller
                        Controller.WinUsbDevice.Dispose();
                    }
                    catch { }

                    Controller.WinUsbDevice = null;
                }

                //Close and dispose the Hid Usb Device
                if (Controller.HidDevice != null)
                {
                    //Disconnect controller from bluetooth
                    try
                    {
                        if (Controller.Connected.Wireless) { Controller.HidDevice.DisconnectBluetooth(); }
                    }
                    catch { Debug.WriteLine("Failed disconnecting device from bluetooth."); }

                    //Dispose and stop connection with the controller
                    try
                    {
                        Controller.HidDevice.CloseDevice();
                    }
                    catch { Debug.WriteLine("Failed disposing and stopping the controller."); }

                    Controller.HidDevice = null;
                }

                //Dispose the connected controller
                Controller.Connected = null;

                Debug.WriteLine("Succesfully stopped the direct input controller.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed stopping the controller direct input: " + ex.Message);
                return false;
            }
        }

        //Stop all the controllers
        async Task StopAllControllers()
        {
            try
            {
                await StopController(vController0, true);
                await StopController(vController1, true);
                await StopController(vController2, true);
                await StopController(vController3, true);

                Debug.WriteLine("Stopped all the controllers direct input.");
                AVActions.ActionDispatcherInvoke(delegate
                {
                    txt_Controller_Information.Text = "Disconnected all the connected controllers.";
                });
            }
            catch { Debug.WriteLine("Failed stopping all controller direct input."); }
        }

        //Open the desired controller
        async Task<bool> OpenController(ControllerStatus Controller)
        {
            try
            {
                //Find and connect to win controller
                if (Controller.Connected.Type == "Win")
                {
                    Controller.WinUsbDevice = new WinUsbDevice();
                    if (!Controller.WinUsbDevice.OpenDevicePath(Controller.Connected.Path, true))
                    {
                        Debug.WriteLine("Invalid winusb device: " + Controller.Connected.DisplayName);
                        vControllerBlockedPaths.Add(Controller.Connected.Path);
                        return false;
                    }
                    else
                    {
                        Controller.InputHeaderByteOffset = 0;
                        Controller.InputReport = new byte[Controller.WinUsbDevice.IntIn];
                        Controller.OutputReport = new byte[Controller.WinUsbDevice.IntOut];

                        Debug.WriteLine("Opened the winusb controller: " + Controller.Connected.DisplayName);
                        vControllerBlockedPaths.Remove(Controller.Connected.Path);
                        return true;
                    }
                }
                //Find and connect to hid controller
                else
                {
                    Controller.HidDevice = new HidDevice(Controller.Connected.Path, Controller.Connected.DisplayName, Controller.Connected.HardwareId);

                    //Disable and enable device to allow exclusive
                    bool DisabledDevice = Controller.HidDevice.DisableDevice();
                    bool EnabledDevice = Controller.HidDevice.EnableDevice();

                    if (!DisabledDevice && !EnabledDevice)
                    {
                        Debug.WriteLine("Device no longer connected: " + Controller.Connected.DisplayName);
                        return false;
                    }
                    else if (!Controller.HidDevice.OpenDeviceExclusively())
                    {
                        Debug.WriteLine("Invalid hid device: " + Controller.Connected.DisplayName);
                        vControllerBlockedPaths.Add(Controller.Connected.Path);
                        return false;
                    }
                    else
                    {
                        Controller.InputHeaderByteOffset = 0;
                        Controller.InputReport = new byte[Controller.HidDevice.Capabilities.InputReportByteLength];
                        Controller.OutputReport = new byte[Controller.HidDevice.Capabilities.OutputReportByteLength];

                        //Read data from the controller
                        bool ReadFile = await Controller.HidDevice.ReadFileTimeout(Controller.InputReport, (uint)Controller.InputReport.Length, IntPtr.Zero, Controller.MilliSecondsReadTime);

                        //Check if the controller connected
                        if (!ReadFile)
                        {
                            Debug.WriteLine("Invalid hid device: " + Controller.Connected.DisplayName + " Len" + Controller.InputReport.Length + " Read" + ReadFile);
                            vControllerBlockedPaths.Add(Controller.Connected.Path);
                            return false;
                        }
                        else if (!Controller.InputReport.Take(5).Any(x => x != 0))
                        {
                            Debug.WriteLine("Invalid hid data: " + Controller.Connected.DisplayName + " Len" + Controller.InputReport.Length + " Read" + ReadFile);
                            return false;
                        }
                        else
                        {
                            Debug.WriteLine("Opened the hid controller: " + Controller.Connected.DisplayName);
                            vControllerBlockedPaths.Remove(Controller.Connected.Path);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("Failed open controller: " + ex.Message); }
            return false;
        }

        //Open the Xbox 360 bus driver
        async Task<bool> OpenX360Bus(ControllerStatus Controller)
        {
            try
            {
                Controller.X360Device = new WinUsbDevice("{F679F562-3164-42CE-A4DB-E7DDBE723909}");
                if (Controller.X360Device.OpenDeviceClass(false))
                {
                    Controller.X360Device.Unplug(Controller.NumberId);
                    await Task.Delay(500);
                    Controller.X360Device.Plugin(Controller.NumberId);
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Controller_Information.Text = "Please make sure that you have installed the required drivers.";
                    });

                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        await Message_InstallDrivers();
                    });

                    return false;
                }
            }
            catch { }
            return true;
        }

        //Check xbox bus driver status
        async Task CheckX360Bus()
        {
            try
            {
                WinUsbDevice X360Device = new WinUsbDevice("{F679F562-3164-42CE-A4DB-E7DDBE723909}");
                if (X360Device.OpenDeviceClass(false))
                {
                    X360Device.UnplugAll();
                    await Task.Delay(500);
                    X360Device.Dispose();
                    X360Device = null;
                }
                else
                {
                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        if (!ShowInTaskbar) { Application_ShowHideWindow(); }
                        await Message_InstallDrivers();
                    });
                }
            }
            catch { }
        }
    }
}