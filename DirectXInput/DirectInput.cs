using ArnoldVinkCode;
using LibraryUsb;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Start Monitoring Direct Input Controllers
        async Task<bool> StartControllerDirectInput(ControllerStatus Controller)
        {
            try
            {
                if (Controller.Connected)
                {
                    Debug.WriteLine("Initializing direct input for: " + Controller.Details.DisplayName);

                    //Allow the controller in HidGuardian
                    HidGuardianAllowController(Controller.Details);
                    await Task.Delay(500);

                    //Open the selected controller
                    if (!await OpenController(Controller))
                    {
                        Debug.WriteLine("Failed to initialize direct input for: " + Controller.Details.DisplayName);

                        NotificationDetails notificationDetailsDisconnected = new NotificationDetails();
                        notificationDetailsDisconnected.Icon = "Controller";
                        notificationDetailsDisconnected.Text = "Controller disconnected";
                        App.vWindowOverlay.Notification_Show_Status(notificationDetailsDisconnected);

                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            txt_Controller_Information.Text = "The controller is no longer connected or supported.";
                        });

                        await StopControllerAsync(Controller, false, "unsupported");
                        return false;
                    }

                    //Open Xbox bus driver
                    if (!await OpenXboxBusDriver(Controller))
                    {
                        Debug.WriteLine("Failed to open Xbox bus driver for: " + Controller.Details.DisplayName);

                        NotificationDetails notificationDetailsDrivers = new NotificationDetails();
                        notificationDetailsDrivers.Icon = "Controller";
                        notificationDetailsDrivers.Text = "Install drivers";
                        App.vWindowOverlay.Notification_Show_Status(notificationDetailsDrivers);

                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            txt_Controller_Information.Text = "Please make sure that you have installed the required drivers.";
                        });

                        return false;
                    }

                    //Set controller interface information
                    string controllerNumberDisplay = (Controller.NumberId + 1).ToString();

                    NotificationDetails notificationDetailsConnected = new NotificationDetails();
                    notificationDetailsConnected.Icon = "Controller";
                    notificationDetailsConnected.Text = "Connected (" + controllerNumberDisplay + ")";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetailsConnected);

                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Controller_Information.Text = "Connected controller " + controllerNumberDisplay + ": " + Controller.Details.DisplayName;
                    });

                    //Update the controller interface settings
                    ControllerUpdateSettingsInterface(Controller);

                    //Update the controller last read time
                    Controller.LastReadTicks = Stopwatch.GetTimestamp();

                    //Update the controller last active time
                    Controller.LastActiveTicks = Environment.TickCount;

                    //Start Translating DirectInput Controller Threads
                    if (Controller.Details.Type == "Win")
                    {
                        async Task TaskAction()
                        {
                            try
                            {
                                await LoopReceiveWinInputData(Controller);
                            }
                            catch { }
                        }
                        AVActions.TaskStartLoop(TaskAction, Controller.InputTask);
                    }
                    else
                    {
                        async Task TaskAction()
                        {
                            try
                            {
                                await LoopReceiveHidInputData(Controller);
                            }
                            catch { }
                        }
                        AVActions.TaskStartLoop(TaskAction, Controller.InputTask);
                    }

                    return true;
                }
            }
            catch { }
            return false;
        }

        //Update the controller interface settings
        void ControllerUpdateSettingsInterface(ControllerStatus Controller)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    cb_ControllerFakeGuideButton.IsChecked = Controller.Details.Profile.FakeGuideButton;

                    cb_ControllerUseButtonTriggers.IsChecked = Controller.Details.Profile.UseButtonTriggers;
                    textblock_ControllerDeadzoneTriggerLeft.Text = textblock_ControllerDeadzoneTriggerLeft.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.DeadzoneTriggerLeft) + "%";
                    slider_ControllerDeadzoneTriggerLeft.Value = Controller.Details.Profile.DeadzoneTriggerLeft;
                    textblock_ControllerDeadzoneTriggerRight.Text = textblock_ControllerDeadzoneTriggerRight.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.DeadzoneTriggerRight) + "%";
                    slider_ControllerDeadzoneTriggerRight.Value = Controller.Details.Profile.DeadzoneTriggerRight;
                    textblock_ControllerSensitivityTrigger.Text = textblock_ControllerSensitivityTrigger.Tag.ToString() + Controller.Details.Profile.SensitivityTrigger.ToString("0.00");
                    slider_ControllerSensitivityTrigger.Value = Controller.Details.Profile.SensitivityTrigger;

                    cb_ControllerDPadFourWayMovement.IsChecked = Controller.Details.Profile.DPadFourWayMovement;

                    cb_ControllerThumbFlipMovement.IsChecked = Controller.Details.Profile.ThumbFlipMovement;
                    cb_ControllerThumbFlipAxesLeft.IsChecked = Controller.Details.Profile.ThumbFlipAxesLeft;
                    cb_ControllerThumbFlipAxesRight.IsChecked = Controller.Details.Profile.ThumbFlipAxesRight;
                    cb_ControllerThumbReverseAxesLeft.IsChecked = Controller.Details.Profile.ThumbReverseAxesLeft;
                    cb_ControllerThumbReverseAxesRight.IsChecked = Controller.Details.Profile.ThumbReverseAxesRight;
                    textblock_ControllerDeadzoneThumbLeft.Text = textblock_ControllerDeadzoneThumbLeft.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.DeadzoneThumbLeft) + "%";
                    slider_ControllerDeadzoneThumbLeft.Value = Controller.Details.Profile.DeadzoneThumbLeft;
                    textblock_ControllerDeadzoneThumbRight.Text = textblock_ControllerDeadzoneThumbRight.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.DeadzoneThumbRight) + "%";
                    slider_ControllerDeadzoneThumbRight.Value = Controller.Details.Profile.DeadzoneThumbRight;
                    textblock_ControllerSensitivityThumb.Text = textblock_ControllerSensitivityThumb.Tag.ToString() + Controller.Details.Profile.SensitivityThumb.ToString("0.00");
                    slider_ControllerSensitivityThumb.Value = Controller.Details.Profile.SensitivityThumb;

                    textblock_ControllerRumbleStrength.Text = textblock_ControllerRumbleStrength.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.RumbleStrength) + "%";
                    slider_ControllerRumbleStrength.Value = Controller.Details.Profile.RumbleStrength;
                    textblock_ControllerLedBrightness.Text = textblock_ControllerLedBrightness.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.LedBrightness) + "%";
                    slider_ControllerLedBrightness.Value = Controller.Details.Profile.LedBrightness;
                });
            }
            catch { }
        }

        //Stop the desired controller in task
        void StopControllerTask(ControllerStatus Controller, bool removeAll360Bus, string disconnectTag)
        {
            try
            {
                //Start the controller stop task
                async void TaskAction()
                {
                    try
                    {
                        await StopControllerAsync(Controller, removeAll360Bus, disconnectTag);
                    }
                    catch { }
                }
                AVActions.TaskStart(TaskAction);
            }
            catch { }
        }

        //Stop the desired controller as async
        async Task<bool> StopControllerAsync(ControllerStatus Controller, bool removeAll360Bus, string disconnectTag)
        {
            try
            {
                //Check if the controller is connected
                if (Controller == null || !Controller.Connected)
                {
                    Debug.WriteLine("Controller is already disconnected.");
                    return false;
                }

                //Update controller block status
                Controller.BlockOutput = true;

                //Update last disconnect time
                vControllerLastDisconnect = DateTime.Now;

                //Get controller display number
                Debug.WriteLine("Disconnecting the controller " + Controller.NumberId + ": " + Controller.Details.DisplayName);
                string controllerNumberDisplay = (Controller.NumberId + 1).ToString();

                //Show controller disconnect notification
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Controller";
                if (string.IsNullOrWhiteSpace(disconnectTag))
                {
                    notificationDetails.Text = "Disconnected (" + controllerNumberDisplay + ")";
                }
                else
                {
                    notificationDetails.Text = "Disconnected " + disconnectTag + " (" + controllerNumberDisplay + ")";
                }
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Update user interface controller status
                AVActions.ActionDispatcherInvoke(delegate
                {
                    txt_Controller_Information.Text = "Disconnected controller " + controllerNumberDisplay + ": " + Controller.Details.DisplayName;
                    if (Controller.NumberId == 0)
                    {
                        image_Controller0.Source = vImagePreloadIconControllerDark;
                        textblock_Controller0.Text = "No controller connected";
                    }
                    else if (Controller.NumberId == 1)
                    {
                        image_Controller1.Source = vImagePreloadIconControllerDark;
                        textblock_Controller1.Text = "No controller connected";
                    }
                    else if (Controller.NumberId == 2)
                    {
                        image_Controller2.Source = vImagePreloadIconControllerDark;
                        textblock_Controller2.Text = "No controller connected";
                    }
                    else if (Controller.NumberId == 3)
                    {
                        image_Controller3.Source = vImagePreloadIconControllerDark;
                        textblock_Controller3.Text = "No controller connected";
                    }
                });

                //Stop the controller loop task
                await TaskStopLoop(Controller.InputTask);

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
                    if (removeAll360Bus) { Controller.X360Device.UnplugAll(); }
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

                //Check if any controller is connected
                if (!vControllerAnyConnected() && Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "KeyboardCloseNoController")))
                {
                    Debug.WriteLine("No controller connected closing keyboard.");
                    await KeyboardControllerHide();
                }

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
                await StopControllerAsync(vController0, true, "all");
                await StopControllerAsync(vController1, true, "all");
                await StopControllerAsync(vController2, true, "all");
                await StopControllerAsync(vController3, true, "all");

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
                        Debug.WriteLine("Connecting device no longer connected: " + Controller.Details.DisplayName);
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
                        bool ReadFile = await Controller.HidDevice.ReadFileTimeout(Controller.InputReport, (uint)Controller.InputReport.Length, IntPtr.Zero, Controller.MilliSecondsAllowRead);

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