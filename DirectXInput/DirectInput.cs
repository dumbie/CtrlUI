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

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Start Monitoring DirectInput Controllers
        async Task<bool> StartControllerDirectInput(ControllerStatus Controller)
        {
            try
            {
                //Check if controller is connected
                if (!Controller.Connected())
                {
                    Debug.WriteLine("DirectInput controller is not connected: " + Controller.Details.DisplayName);
                    return false;
                }

                Debug.WriteLine("Initializing DirectInput for: " + Controller.Details.DisplayName);

                //Allow controller in HidHide
                if (Controller.Details.Type == ControllerType.HidDevice)
                {
                    await vHidHideDevice.ListDeviceAdd(Controller.Details.DeviceInstanceId);
                }

                //Set controller interface information
                string controllerNumberDisplay = (Controller.NumberId + 1).ToString();

                //Open the selected controller
                if (!OpenController(Controller))
                {
                    Debug.WriteLine("Failed to initialize DirectInput for: " + Controller.Details.DisplayName);
                    await StopController(Controller, "failed", "Controller " + controllerNumberDisplay + " is no longer connected or failed.");
                    return false;
                }

                //Unplug and plugin the virtual device
                await vVirtualBusDevice.VirtualUnplug(Controller.NumberId);
                await vVirtualBusDevice.VirtualPlugin(Controller.NumberId);

                NotificationDetails notificationDetailsConnected = new NotificationDetails();
                notificationDetailsConnected.Icon = "Controller";
                notificationDetailsConnected.Text = "Connected (" + controllerNumberDisplay + ")";
                notificationDetailsConnected.Color = Controller.Color;
                vWindowOverlay.Notification_Show_Status(notificationDetailsConnected);

                AVActions.DispatcherInvoke(delegate
                {
                    txt_Controller_Information.Text = "Connected controller " + controllerNumberDisplay + ": " + Controller.Details.DisplayName;
                });

                //Update the controller interface settings
                ControllerUpdateSettingsInterface(Controller);

                //Update the controller last input time
                long ticksSystem = GetSystemTicksMs();
                Controller.TicksInputPrev = ticksSystem;
                Controller.TicksInputLast = ticksSystem;

                //Update the controller last active time
                Controller.TicksActiveLast = ticksSystem;

                //Set the controller supported profile
                Controller.SupportedCurrent = vDirectControllersSupported.FirstOrDefault(x => x.ProductIDs.Any(z => z.ToLower() == Controller.Details.Profile.ProductID.ToLower() && x.VendorID.ToLower() == Controller.Details.Profile.VendorID.ToLower()));
                if (Controller.SupportedCurrent == null)
                {
                    Debug.WriteLine("Unsupported controller detected, using default profile.");
                    Controller.SupportedCurrent = new ControllerSupported();
                }

                //Initialize controller
                ControllerInitialize(Controller);

                //Controller update led color
                ControllerLedColor(Controller);

                //Start input controller task loop
                async Task TaskActionInputController()
                {
                    try
                    {
                        await LoopInputController(Controller);
                    }
                    catch { }
                }
                AVActions.TaskStartLoop(TaskActionInputController, Controller.InputControllerTask);

                //Start output controller task loop
                void TaskActionOutputController()
                {
                    try
                    {
                        LoopOutputController(Controller);
                    }
                    catch { }
                }
                AVActions.TaskStartLoop(TaskActionOutputController, Controller.OutputControllerTask);

                //Start output gyroscope task loop
                if (Controller.SupportedCurrent.OffsetHeader.Gyroscope != null)
                {
                    async Task TaskActionOutputGyro()
                    {
                        try
                        {
                            await LoopOutputGyro(Controller);
                        }
                        catch { }
                    }
                    AVActions.TaskStartLoop(TaskActionOutputGyro, Controller.OutputGyroscopeTask);
                }

                return true;
            }
            catch
            {
                Debug.WriteLine("Failed initializing DirectInput for: " + Controller.Details.DisplayName);
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
                    //Check if controller supports rumble mode
                    if (Controller.SupportedCurrent.HasRumbleMode)
                    {
                        stackpanel_ControllerRumbleMode.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        stackpanel_ControllerRumbleMode.Visibility = Visibility.Collapsed;
                    }

                    //Check if controller supports trigger rumble
                    if (Controller.SupportedCurrent.HasRumbleTrigger)
                    {
                        stackpanel_TriggerRumbleSettings.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        stackpanel_TriggerRumbleSettings.Visibility = Visibility.Collapsed;
                    }

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

                    //Thumb deadzone
                    textblock_ControllerDeadzoneThumbLeft.Text = textblock_ControllerDeadzoneThumbLeft.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.DeadzoneThumbLeft) + "%";
                    slider_ControllerDeadzoneThumbLeft.Value = Controller.Details.Profile.DeadzoneThumbLeft;
                    textblock_ControllerDeadzoneThumbRight.Text = textblock_ControllerDeadzoneThumbRight.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.DeadzoneThumbRight) + "%";
                    slider_ControllerDeadzoneThumbRight.Value = Controller.Details.Profile.DeadzoneThumbRight;

                    //Thumb sensitivity
                    textblock_ControllerSensitivityThumbLeft.Text = textblock_ControllerSensitivityThumbLeft.Tag.ToString() + Controller.Details.Profile.SensitivityThumbLeft.ToString("0.00");
                    slider_ControllerSensitivityThumbLeft.Value = Controller.Details.Profile.SensitivityThumbLeft;
                    textblock_ControllerSensitivityThumbRight.Text = textblock_ControllerSensitivityThumbRight.Tag.ToString() + Controller.Details.Profile.SensitivityThumbRight.ToString("0.00");
                    slider_ControllerSensitivityThumbRight.Value = Controller.Details.Profile.SensitivityThumbRight;

                    cb_ControllerRumbleEnabled.IsChecked = Controller.Details.Profile.ControllerRumbleEnabled;
                    combobox_ControllerRumbleMode.SelectedIndex = Controller.Details.Profile.ControllerRumbleMode;
                    if (Controller.Details.Profile.ControllerRumbleEnabled)
                    {
                        combobox_ControllerRumbleMode.IsEnabled = true;
                        slider_ControllerRumbleStrength.IsEnabled = true;
                        slider_ControllerRumbleLimit.IsEnabled = true;
                    }
                    else
                    {
                        combobox_ControllerRumbleMode.IsEnabled = false;
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

                //Disconnect gyroscope dsu
                if (controller.SupportedCurrent.OffsetHeader.Gyroscope != null)
                {
                    //Stop gyroscope loop tasks
                    await TaskStopLoop(controller.OutputGyroscopeTask, 1000);
                }

                //Disconnect virtual controller
                if (vVirtualBusDevice != null)
                {
                    //Disconnect the virtual controller
                    await vVirtualBusDevice.VirtualUnplug(controller.NumberId);
                }

                //Disconnect Hid or WinUsb device
                if (controller.WinUsbDevice != null)
                {
                    //Stop controller device loop tasks
                    await TaskStopLoop(controller.InputControllerTask, 1000);
                    await TaskStopLoop(controller.OutputControllerTask, 1000);

                    //Dispose and stop connection with the controller
                    try
                    {
                        controller.WinUsbDevice.CloseDevice();
                    }
                    catch { }
                }
                else if (controller.HidDevice != null)
                {
                    //Stop controller device loop tasks
                    await TaskStopLoop(controller.InputControllerTask, 1000);
                    await TaskStopLoop(controller.OutputControllerTask, 1000);

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

        //Open the desired controller
        bool OpenController(ControllerStatus Controller)
        {
            try
            {
                //Find and connect to win controller
                if (Controller.Details.Type == ControllerType.WinUsbDevice)
                {
                    Controller.WinUsbDevice = new WinUsbDevice(Controller.Details.DevicePath, Controller.Details.DeviceInstanceId, true, false);
                    if (!Controller.WinUsbDevice.Connected)
                    {
                        Debug.WriteLine("Invalid winusb open device: " + Controller.Details.DisplayName);
                        return false;
                    }
                    else
                    {
                        //Set default controller variables
                        Controller.ControllerDataInput = new byte[Controller.WinUsbDevice.IntIn];
                        Controller.ControllerDataOutput = new byte[Controller.WinUsbDevice.IntOut];

                        Debug.WriteLine("Opened the winusb controller: " + Controller.Details.DisplayName);
                        return true;
                    }
                }
                //Find and connect to hid controller
                else
                {
                    Controller.HidDevice = new HidDevice(Controller.Details.DevicePath, Controller.Details.DeviceInstanceId, true, false);
                    if (!Controller.HidDevice.Connected)
                    {
                        Debug.WriteLine("Invalid hid open device: " + Controller.Details.DisplayName);
                        return false;
                    }
                    else
                    {
                        //Set default controller variables
                        Controller.ControllerDataInput = new byte[Controller.HidDevice.Capabilities.InputReportByteLength];
                        Controller.ControllerDataOutput = new byte[Controller.HidDevice.Capabilities.OutputReportByteLength];

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