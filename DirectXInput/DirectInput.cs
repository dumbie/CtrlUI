using ArnoldVinkCode;
using LibraryUsb;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryUsb.Events;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Start Monitoring Direct Input Controllers
        async Task<bool> StartControllerDirectInput(ControllerStatus Controller)
        {
            try
            {
                //Check if controller is connected
                if (!Controller.Connected())
                {
                    Debug.WriteLine("Direct input controller is not connected: " + Controller.Details.DisplayName);
                    return false;
                }

                Debug.WriteLine("Initializing direct input for: " + Controller.Details.DisplayName);

                //Allow controller in HidHide
                if (Controller.Details.Type == ControllerType.HidDevice)
                {
                    await vHidHideDevice.ListDeviceAdd(Controller.Details.ModelId);
                }

                //Set controller interface information
                string controllerNumberDisplay = (Controller.NumberId + 1).ToString();

                //Open the selected controller
                if (!OpenController(Controller))
                {
                    Debug.WriteLine("Failed to initialize direct input for: " + Controller.Details.DisplayName);
                    await StopController(Controller, "failed", "Controller " + controllerNumberDisplay + " is no longer connected or failed.");
                    return false;
                }

                //Unplug and plugin the virtual device
                vVirtualBusDevice.VirtualUnplug(Controller.NumberId);
                await Task.Delay(500);
                vVirtualBusDevice.VirtualPlugin(Controller.NumberId);

                NotificationDetails notificationDetailsConnected = new NotificationDetails();
                notificationDetailsConnected.Icon = "Controller";
                notificationDetailsConnected.Text = "Connected (" + controllerNumberDisplay + ")";
                notificationDetailsConnected.Color = Controller.Color;
                App.vWindowOverlay.Notification_Show_Status(notificationDetailsConnected);

                AVActions.DispatcherInvoke(delegate
                {
                    txt_Controller_Information.Text = "Connected controller " + controllerNumberDisplay + ": " + Controller.Details.DisplayName;
                });

                //Update the controller interface settings
                ControllerUpdateSettingsInterface(Controller);

                //Update the controller last read time
                Controller.PrevInputTicks = GetSystemTicksMs();
                Controller.LastInputTicks = GetSystemTicksMs();

                //Update the controller last active time
                Controller.LastActiveTicks = GetSystemTicksMs();

                //Set the controller supported profile
                Controller.SupportedCurrent = vDirectControllersSupported.Where(x => x.ProductIDs.Any(z => z.ToLower() == Controller.Details.Profile.ProductID.ToLower() && x.VendorID.ToLower() == Controller.Details.Profile.VendorID.ToLower())).FirstOrDefault();
                if (Controller.SupportedCurrent == null)
                {
                    Debug.WriteLine("Unsupported controller detected, using default profile.");
                    Controller.SupportedCurrent = new ControllerSupported();
                }

                //Start controller input task loop
                async Task TaskActionInput()
                {
                    try
                    {
                        await LoopInputDirectInput(Controller, Controller.Details.Type);
                    }
                    catch { }
                }
                AVActions.TaskStartLoop(TaskActionInput, Controller.InputControllerTask);

                //Start virtual output task loop
                void TaskActionOutputVirtual()
                {
                    try
                    {
                        LoopOutputVirtual(Controller);
                    }
                    catch { }
                }
                AVActions.TaskStartLoop(TaskActionOutputVirtual, Controller.OutputVirtualTask);

                //Start controller output task loop
                void TaskActionOutputController()
                {
                    try
                    {
                        LoopOutputController(Controller);
                    }
                    catch { }
                }
                AVActions.TaskStartLoop(TaskActionOutputController, Controller.OutputControllerTask);

                //Start gyroscope task loop
                if (Controller.SupportedCurrent.HasGyroscope)
                {
                    async Task TaskActionOutputGyro()
                    {
                        try
                        {
                            await LoopOutputGyro(Controller);
                        }
                        catch { }
                    }
                    AVActions.TaskStartLoop(TaskActionOutputGyro, Controller.OutputGyroTask);
                }

                return true;
            }
            catch
            {
                Debug.WriteLine("Failed initializing direct input for: " + Controller.Details.DisplayName);
                return false;
            }
        }

        //Update the controller interface settings
        public void ControllerUpdateSettingsInterface(ControllerStatus Controller)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Check if controller supports trigger rumble
                    if (Controller.SupportedCurrent.HasRumbleTrigger)
                    {
                        stackpanel_TriggerRumbleSettings.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        stackpanel_TriggerRumbleSettings.Visibility = Visibility.Collapsed;
                    }

                    cb_ControllerFakeGuideButton.IsChecked = Controller.Details.Profile.FakeGuideButton;
                    cb_ControllerFakeMediaButton.IsChecked = Controller.Details.Profile.FakeMediaButton;
                    cb_ControllerFakeTouchpadButton.IsChecked = Controller.Details.Profile.FakeTouchpadButton;

                    cb_ControllerUseButtonTriggers.IsChecked = Controller.Details.Profile.UseButtonTriggers;
                    textblock_ControllerDeadzoneTriggerLeft.Text = textblock_ControllerDeadzoneTriggerLeft.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.DeadzoneTriggerLeft) + "%";
                    slider_ControllerDeadzoneTriggerLeft.Value = Controller.Details.Profile.DeadzoneTriggerLeft;
                    textblock_ControllerDeadzoneTriggerRight.Text = textblock_ControllerDeadzoneTriggerRight.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.DeadzoneTriggerRight) + "%";
                    slider_ControllerDeadzoneTriggerRight.Value = Controller.Details.Profile.DeadzoneTriggerRight;
                    textblock_ControllerSensitivityTriggerLeft.Text = textblock_ControllerSensitivityTriggerLeft.Tag.ToString() + Controller.Details.Profile.SensitivityTriggerLeft.ToString("0.00");
                    slider_ControllerSensitivityTriggerLeft.Value = Controller.Details.Profile.SensitivityTriggerLeft;
                    textblock_ControllerSensitivityTriggerRight.Text = textblock_ControllerSensitivityTriggerRight.Tag.ToString() + Controller.Details.Profile.SensitivityTriggerRight.ToString("0.00");
                    slider_ControllerSensitivityTriggerRight.Value = Controller.Details.Profile.SensitivityTriggerRight;

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

                    cb_ControllerRumbleEnabled.IsChecked = Controller.Details.Profile.ControllerRumbleEnabled;
                    if (Controller.Details.Profile.ControllerRumbleEnabled)
                    {
                        slider_ControllerRumbleStrength.IsEnabled = true;
                        slider_ControllerRumbleLimit.IsEnabled = true;
                    }
                    else
                    {
                        slider_ControllerRumbleStrength.IsEnabled = false;
                        slider_ControllerRumbleLimit.IsEnabled = false;
                    }

                    textblock_ControllerRumbleLimit.Text = textblock_ControllerRumbleLimit.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.ControllerRumbleLimit) + "%";
                    slider_ControllerRumbleLimit.Value = Controller.Details.Profile.ControllerRumbleLimit;

                    textblock_ControllerRumbleStrength.Text = textblock_ControllerRumbleStrength.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.ControllerRumbleStrength) + "%";
                    slider_ControllerRumbleStrength.Value = Controller.Details.Profile.ControllerRumbleStrength;

                    cb_TriggerRumbleEnabled.IsChecked = Controller.Details.Profile.TriggerRumbleEnabled;
                    if (Controller.Details.Profile.TriggerRumbleEnabled)
                    {
                        slider_TriggerRumbleStrengthLeft.IsEnabled = true;
                        slider_TriggerRumbleStrengthRight.IsEnabled = true;
                        slider_TriggerRumbleLimit.IsEnabled = true;
                    }
                    else
                    {
                        slider_TriggerRumbleStrengthLeft.IsEnabled = false;
                        slider_TriggerRumbleStrengthRight.IsEnabled = false;
                        slider_TriggerRumbleLimit.IsEnabled = false;
                    }

                    textblock_TriggerRumbleLimit.Text = textblock_TriggerRumbleLimit.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.TriggerRumbleLimit) + "%";
                    slider_TriggerRumbleLimit.Value = Controller.Details.Profile.TriggerRumbleLimit;

                    textblock_TriggerRumbleStrengthLeft.Text = textblock_TriggerRumbleStrengthLeft.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.TriggerRumbleStrengthLeft) + "%";
                    slider_TriggerRumbleStrengthLeft.Value = Controller.Details.Profile.TriggerRumbleStrengthLeft;

                    textblock_TriggerRumbleStrengthRight.Text = textblock_TriggerRumbleStrengthRight.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.TriggerRumbleStrengthRight) + "%";
                    slider_TriggerRumbleStrengthRight.Value = Controller.Details.Profile.TriggerRumbleStrengthRight;

                    textblock_ControllerLedBrightness.Text = textblock_ControllerLedBrightness.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.LedBrightness) + "%";
                    slider_ControllerLedBrightness.Value = Controller.Details.Profile.LedBrightness;
                });
            }
            catch { }
        }

        //Stop the desired controller
        private async Task<bool> StopController(ControllerStatus Controller, string disconnectInfo, string controllerInfo)
        {
            try
            {
                //Check if the controller is connected
                if (Controller == null || !Controller.Connected())
                {
                    Debug.WriteLine("Controller " + Controller.NumberId + " is already disconnected.");
                    return false;
                }

                //Check if the controller is disconnecting
                if (Controller.Disconnecting)
                {
                    Debug.WriteLine("Controller " + Controller.NumberId + " is currently disconnecting.");
                    return false;
                }

                //Update controller disconnecting status
                Controller.Disconnecting = true;

                //Get controller display number
                Debug.WriteLine("Disconnecting the controller " + Controller.NumberId + ": " + Controller.Details.DisplayName);
                string controllerNumberDisplay = (Controller.NumberId + 1).ToString();

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
                notificationDetails.Color = Controller.Color;
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Update user interface controller status
                AVActions.DispatcherInvoke(delegate
                {
                    if (string.IsNullOrWhiteSpace(controllerInfo))
                    {
                        txt_Controller_Information.Text = "Disconnected controller " + controllerNumberDisplay + ": " + Controller.Details.DisplayName;
                    }
                    else
                    {
                        txt_Controller_Information.Text = controllerInfo;
                    }

                    if (Controller.NumberId == 0)
                    {
                        image_Controller0.Source = vImagePreloadIconControllerDark;
                        textblock_Controller0.Text = "No controller connected";
                        textblock_Controller0CodeName.Text = string.Empty;
                        ResetControllerDebugInformation();
                    }
                    else if (Controller.NumberId == 1)
                    {
                        image_Controller1.Source = vImagePreloadIconControllerDark;
                        textblock_Controller1.Text = "No controller connected";
                        textblock_Controller1CodeName.Text = string.Empty;
                        ResetControllerDebugInformation();
                    }
                    else if (Controller.NumberId == 2)
                    {
                        image_Controller2.Source = vImagePreloadIconControllerDark;
                        textblock_Controller2.Text = "No controller connected";
                        textblock_Controller2CodeName.Text = string.Empty;
                        ResetControllerDebugInformation();
                    }
                    else if (Controller.NumberId == 3)
                    {
                        image_Controller3.Source = vImagePreloadIconControllerDark;
                        textblock_Controller3.Text = "No controller connected";
                        textblock_Controller3CodeName.Text = string.Empty;
                        ResetControllerDebugInformation();
                    }
                });

                //Disconnect gyro dsu
                if (Controller.SupportedCurrent.HasGyroscope)
                {
                    //Stop gyro controller loop task
                    await TaskStopLoop(Controller.OutputGyroTask, 1000);

                    //Send empty input to gyro dsu
                    await SendGyroMotionEmpty(Controller);
                }

                //Disconnect virtual controller
                if (vVirtualBusDevice != null)
                {
                    //Stop virtual controller loop task
                    await TaskStopLoop(Controller.OutputVirtualTask, 1000);

                    //Send empty input to virtual device
                    SendInputVirtualEmpty(Controller);

                    //Close the controller virtual events
                    SetAndCloseEvent(Controller.InputVirtualOverlapped.EventHandle);
                    SetAndCloseEvent(Controller.OutputVirtualOverlapped.EventHandle);

                    //Disconnect the virtual controller
                    vVirtualBusDevice.VirtualUnplug(Controller.NumberId);
                }

                //Disconnect Hid or WinUsb Device
                if (Controller.WinUsbDevice != null)
                {
                    //Stop controller device loop tasks
                    await TaskStopLoop(Controller.InputControllerTask, 1000);
                    await TaskStopLoop(Controller.OutputControllerTask, 1000);

                    //Dispose and stop connection with the controller
                    try
                    {
                        Controller.WinUsbDevice.CloseDevice();
                    }
                    catch { }
                }
                else if (Controller.HidDevice != null)
                {
                    //Stop controller device loop tasks
                    await TaskStopLoop(Controller.InputControllerTask, 1000);
                    await TaskStopLoop(Controller.OutputControllerTask, 1000);

                    //Disconnect controller from bluetooth
                    if (Controller.Details.Wireless)
                    {
                        try
                        {
                            Controller.HidDevice.BluetoothDisconnect();
                        }
                        catch
                        {
                            Debug.WriteLine("Failed disconnecting device from bluetooth.");
                        }
                    }

                    //Signal Windows disconnection to prevent ghost controller
                    if (Controller.Details.Wireless)
                    {
                        try
                        {
                            Controller.HidDevice.GetFeature(0x02);
                            Controller.HidDevice.GetFeature(0x05);
                        }
                        catch
                        {
                            Debug.WriteLine("Failed signaling controller disconnection to Windows.");
                        }
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
                }

                //Reset the controller status
                Controller.ResetControllerStatus();

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

                Debug.WriteLine("Succesfully stopped direct input controller " + Controller.NumberId);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed stopping the controller direct input " + Controller.NumberId + ": " + ex.Message);
                return false;
            }
        }

        //Stop all the controllers
        async Task StopAllControllers(bool disconnectVirtualBus)
        {
            try
            {
                await StopController(vController0, "all", "Disconnected all controllers.");
                await StopController(vController1, "all", "Disconnected all controllers.");
                await StopController(vController2, "all", "Disconnected all controllers.");
                await StopController(vController3, "all", "Disconnected all controllers.");

                if (disconnectVirtualBus)
                {
                    vVirtualBusDevice.CloseDevice();
                    vVirtualBusDevice = null;
                }

                Debug.WriteLine("Stopped all the controllers direct input.");
            }
            catch
            {
                Debug.WriteLine("Failed stopping all controller direct input.");
            }
        }

        //Open the desired controller
        bool OpenController(ControllerStatus Controller)
        {
            try
            {
                //Find and connect to win controller
                if (Controller.Details.Type == ControllerType.WinUsbDevice)
                {
                    Controller.WinUsbDevice = new WinUsbDevice(Guid.Empty, Controller.Details.Path, true, false);
                    if (!Controller.WinUsbDevice.Connected)
                    {
                        Debug.WriteLine("Invalid winusb open device: " + Controller.Details.DisplayName);
                        return false;
                    }
                    else
                    {
                        //Set default controller variables
                        Controller.InputReport = new byte[Controller.WinUsbDevice.IntIn];
                        Controller.OutputReport = new byte[Controller.WinUsbDevice.IntOut];

                        Debug.WriteLine("Opened the winusb controller: " + Controller.Details.DisplayName);
                        return true;
                    }
                }
                //Find and connect to hid controller
                else
                {
                    Controller.HidDevice = new HidDevice(Controller.Details.Path, Controller.Details.ModelId, true, false);
                    if (!Controller.HidDevice.Connected)
                    {
                        Debug.WriteLine("Invalid hid open device: " + Controller.Details.DisplayName);
                        return false;
                    }
                    else
                    {
                        //Set default controller variables
                        Controller.InputReport = new byte[Controller.HidDevice.Capabilities.InputReportByteLength];
                        Controller.OutputReport = new byte[Controller.HidDevice.Capabilities.OutputReportByteLength];

                        Debug.WriteLine("Opened the hid controller: " + Controller.Details.DisplayName + ", exclusive: " + Controller.HidDevice.Exclusive);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed opening controller: " + ex.Message);
            }
            return false;
        }
    }
}