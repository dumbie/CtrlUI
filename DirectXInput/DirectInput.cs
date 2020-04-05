using ArnoldVinkCode;
using LibraryUsb;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
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
                if (Controller.Connected())
                {
                    Debug.WriteLine("Initializing direct input for: " + Controller.Details.DisplayName);

                    //Allow the controller in HidGuardian
                    HidGuardianAllowController(Controller.Details);
                    await Task.Delay(500);

                    //Open the selected controller
                    if (!await OpenController(Controller))
                    {
                        Debug.WriteLine("Failed to initialize direct input for: " + Controller.Details.DisplayName);
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            txt_Controller_Information.Text = "The controller is no longer connected or supported.";
                        });

                        await StopController(Controller, false);
                        return false;
                    }

                    //Open Xbox bus driver
                    if (!await OpenXboxBusDriver(Controller))
                    {
                        Debug.WriteLine("Failed to open Xbox bus driver for: " + Controller.Details.DisplayName);
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            txt_Controller_Information.Text = "Please make sure that you have installed the required drivers.";
                        });

                        return false;
                    }

                    //Set controller interface information
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Controller_Information.Text = "Connected controller " + (Controller.NumberId + 1) + ": " + Controller.Details.DisplayName;
                    });

                    //Update the controller interface settings
                    UpdateControllerSettingsInterface(Controller);

                    //Update the last controller active time
                    Controller.LastActive = Environment.TickCount;

                    //Start Translating DirectInput Controller Threads
                    if (Controller.Details.Type == "Win")
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
                    cb_ControllerFakeGuideButton.IsChecked = Controller.Details.Profile.FakeGuideButton;
                    cb_ControllerUseButtonTriggers.IsChecked = Controller.Details.Profile.UseButtonTriggers;
                    cb_ControllerDPadFourWayMovement.IsChecked = Controller.Details.Profile.DPadFourWayMovement;
                    cb_ControllerThumbFlipMovement.IsChecked = Controller.Details.Profile.ThumbFlipMovement;
                    cb_ControllerThumbFlipAxesLeft.IsChecked = Controller.Details.Profile.ThumbFlipAxesLeft;
                    cb_ControllerThumbFlipAxesRight.IsChecked = Controller.Details.Profile.ThumbFlipAxesRight;
                    cb_ControllerThumbReverseAxesLeft.IsChecked = Controller.Details.Profile.ThumbReverseAxesLeft;
                    cb_ControllerThumbReverseAxesRight.IsChecked = Controller.Details.Profile.ThumbReverseAxesRight;
                    textblock_ControllerRumbleStrength.Text = "Rumble strength: " + Convert.ToInt32(Controller.Details.Profile.RumbleStrength) + "%";
                    slider_ControllerRumbleStrength.Value = Controller.Details.Profile.RumbleStrength;
                    textblock_ControllerLedBrightness.Text = "Led brightness: " + Convert.ToInt32(Controller.Details.Profile.LedBrightness) + "%";
                    slider_ControllerLedBrightness.Value = Controller.Details.Profile.LedBrightness;
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
                if (Controller.Connected())
                {
                    Debug.WriteLine("Disconnecting the controller " + Controller.NumberId + ": " + Controller.Details.DisplayName);
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Controller_Information.Text = "Disconnected controller " + (Controller.NumberId + 1) + ": " + Controller.Details.DisplayName;
                        if (Controller.NumberId == 0)
                        {
                            image_Controller0.Source = new BitmapImage(new Uri("Assets/Icons/Controller-Dark.png", UriKind.Relative));
                            textblock_Controller0.Text = "No controller connected";
                        }
                        else if (Controller.NumberId == 1)
                        {
                            image_Controller1.Source = new BitmapImage(new Uri("Assets/Icons/Controller-Dark.png", UriKind.Relative));
                            textblock_Controller1.Text = "No controller connected";
                        }
                        else if (Controller.NumberId == 2)
                        {
                            image_Controller2.Source = new BitmapImage(new Uri("Assets/Icons/Controller-Dark.png", UriKind.Relative));
                            textblock_Controller2.Text = "No controller connected";
                        }
                        else if (Controller.NumberId == 3)
                        {
                            image_Controller3.Source = new BitmapImage(new Uri("Assets/Icons/Controller-Dark.png", UriKind.Relative));
                            textblock_Controller3.Text = "No controller connected";
                        }
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
                    //Prepare empty XOutput device data
                    PrepareXInputData(Controller, true);

                    //Send XOutput device data
                    Controller.X360Device.Send(Controller.XInputData, Controller.XOutputData);
                    await Task.Delay(500);

                    //Stop and disconnect the x360 device
                    if (RemoveAll360Bus) { Controller.X360Device.UnplugAll(); }
                    Controller.X360Device.Unplug(Controller.NumberId);
                    Controller.X360Device.Dispose();
                    Controller.X360Device = null;

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
                        if (Controller.Details.Wireless) { Controller.HidDevice.DisconnectBluetooth(); }
                    }
                    catch
                    {
                        Debug.WriteLine("Failed disconnecting device from bluetooth.");
                    }

                    //Dispose and stop connection with the controller
                    try
                    {
                        Controller.HidDevice.CloseDevice();
                    }
                    catch
                    {
                        Debug.WriteLine("Failed disposing and stopping the controller.");
                    }

                    Controller.HidDevice = null;
                }

                //Dispose the connected controller
                Controller.Details = null;

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
            catch
            {
                Debug.WriteLine("Failed stopping all controller direct input.");
            }
        }

        //Open the desired controller
        async Task<bool> OpenController(ControllerStatus Controller)
        {
            try
            {
                //Find and connect to win controller
                if (Controller.Details.Type == "Win")
                {
                    Controller.WinUsbDevice = new WinUsbDevice();
                    if (!Controller.WinUsbDevice.OpenDevicePath(Controller.Details.Path, true))
                    {
                        Debug.WriteLine("Invalid winusb device, blocking: " + Controller.Details.DisplayName);
                        vControllerTempBlockPaths.Add(Controller.Details.Path);
                        return false;
                    }
                    else
                    {
                        Controller.InputHeaderByteOffset = 0;
                        Controller.InputReport = new byte[Controller.WinUsbDevice.IntIn];
                        Controller.OutputReport = new byte[Controller.WinUsbDevice.IntOut];

                        Debug.WriteLine("Opened the winusb controller: " + Controller.Details.DisplayName);
                        vControllerTempBlockPaths.Remove(Controller.Details.Path);
                        return true;
                    }
                }
                //Find and connect to hid controller
                else
                {
                    Controller.HidDevice = new HidDevice(Controller.Details.Path, Controller.Details.DisplayName, Controller.Details.HardwareId);

                    //Disable and enable device to allow exclusive
                    bool DisabledDevice = Controller.HidDevice.DisableDevice();
                    bool EnabledDevice = Controller.HidDevice.EnableDevice();

                    if (!DisabledDevice && !EnabledDevice)
                    {
                        Debug.WriteLine("Device no longer connected: " + Controller.Details.DisplayName);
                        return false;
                    }
                    else if (!Controller.HidDevice.OpenDeviceExclusively())
                    {
                        Debug.WriteLine("Invalid exclusive hid device, blocking: " + Controller.Details.DisplayName);
                        vControllerTempBlockPaths.Add(Controller.Details.Path);
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
                            Debug.WriteLine("Invalid hid device, blocking: " + Controller.Details.DisplayName + " Len" + Controller.InputReport.Length + " Read" + ReadFile);
                            vControllerTempBlockPaths.Add(Controller.Details.Path);
                            return false;
                        }
                        else if (!Controller.InputReport.Take(5).Any(x => x != 0))
                        {
                            Debug.WriteLine("Invalid hid data: " + Controller.Details.DisplayName + " Len" + Controller.InputReport.Length + " Read" + ReadFile);
                            return false;
                        }
                        else
                        {
                            Debug.WriteLine("Opened the hid controller: " + Controller.Details.DisplayName);
                            vControllerTempBlockPaths.Remove(Controller.Details.Path);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed opening controller: " + ex.Message);
            }
            return false;
        }

        //Open Xbox bus driver
        async Task<bool> OpenXboxBusDriver(ControllerStatus Controller)
        {
            bool driverOpened = false;
            try
            {
                Controller.X360Device = new WinUsbDevice("{F679F562-3164-42CE-A4DB-E7DDBE723909}");
                if (Controller.X360Device.OpenDeviceClass(false))
                {
                    Controller.X360Device.Unplug(Controller.NumberId);
                    await Task.Delay(500);
                    Controller.X360Device.Plugin(Controller.NumberId);
                    driverOpened = true;
                }
                else
                {
                    driverOpened = false;
                }
            }
            catch { }
            return driverOpened;
        }

        //Check Xbox bus driver status
        async Task<bool> CheckXboxBusDriverStatus()
        {
            bool driverInstalled = false;
            try
            {
                WinUsbDevice X360Device = new WinUsbDevice("{F679F562-3164-42CE-A4DB-E7DDBE723909}");
                if (X360Device.OpenDeviceClass(false))
                {
                    Debug.WriteLine("Xbox drivers are installed.");
                    X360Device.UnplugAll();
                    await Task.Delay(500);
                    X360Device.Dispose();
                    X360Device = null;
                    driverInstalled = true;
                }
                else
                {
                    Debug.WriteLine("Xbox drivers not installed.");
                    driverInstalled = false;
                }
            }
            catch { }
            return driverInstalled;
        }
    }
}