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
                await vHidHideDevice.ListDeviceAdd(Controller.Details.ModelId);

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

                    await StopControllerAsync(Controller, "unsupported");
                    return false;
                }

                //Unplug and plugin the virtual device
                vVirtualBusDevice.VirtualUnplug(Controller.NumberId);
                await Task.Delay(500);
                vVirtualBusDevice.VirtualPlugin(Controller.NumberId);

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
                Controller.PrevInputTicks = Controller.LastInputTicks;
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
                if (Controller.Details.Type == "Win")
                {
                    async Task TaskActionInput()
                    {
                        try
                        {
                            await LoopInputWinUsb(Controller);
                        }
                        catch { }
                    }
                    AVActions.TaskStartLoop(TaskActionInput, Controller.InputControllerTask);
                }
                else
                {
                    async Task TaskActionInput()
                    {
                        try
                        {
                            await LoopInputHidDevice(Controller);
                        }
                        catch { }
                    }
                    AVActions.TaskStartLoop(TaskActionInput, Controller.InputControllerTask);
                }

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

                    cb_ControllerRumbleEnabled.IsChecked = Controller.Details.Profile.ControllerRumbleEnabled;
                    if (Controller.Details.Profile.ControllerRumbleEnabled)
                    {
                        slider_ControllerRumbleStrength.IsEnabled = true;
                    }
                    else
                    {
                        slider_ControllerRumbleStrength.IsEnabled = false;
                    }
                    textblock_ControllerRumbleStrength.Text = textblock_ControllerRumbleStrength.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.ControllerRumbleStrength) + "%";
                    slider_ControllerRumbleStrength.Value = Controller.Details.Profile.ControllerRumbleStrength;

                    cb_TriggerRumbleEnabled.IsChecked = Controller.Details.Profile.TriggerRumbleEnabled;
                    if (Controller.Details.Profile.TriggerRumbleEnabled)
                    {
                        slider_TriggerRumbleStrengthLeft.IsEnabled = true;
                        slider_TriggerRumbleStrengthRight.IsEnabled = true;
                    }
                    else
                    {
                        slider_TriggerRumbleStrengthLeft.IsEnabled = false;
                        slider_TriggerRumbleStrengthRight.IsEnabled = false;
                    }
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

        //Stop the desired controller in task
        void StopControllerTask(ControllerStatus Controller, string disconnectTag)
        {
            try
            {
                //Start the controller stop task
                async void TaskAction()
                {
                    try
                    {
                        await StopControllerAsync(Controller, disconnectTag);
                    }
                    catch { }
                }
                AVActions.TaskStart(TaskAction);
            }
            catch { }
        }

        //Stop the desired controller as async
        async Task<bool> StopControllerAsync(ControllerStatus Controller, string disconnectTag)
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
                if (Controller.BlockInteraction)
                {
                    Debug.WriteLine("Controller " + Controller.NumberId + " is currently disconnecting.");
                    return false;
                }

                //Update controller block status
                Controller.BlockInteraction = true;

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
                        textblock_Controller0CodeName.Text = string.Empty;
                        textblock_LiveDebugInformation.Text = "Connect a controller to show debug information.";
                    }
                    else if (Controller.NumberId == 1)
                    {
                        image_Controller1.Source = vImagePreloadIconControllerDark;
                        textblock_Controller1.Text = "No controller connected";
                        textblock_Controller1CodeName.Text = string.Empty;
                        textblock_LiveDebugInformation.Text = "Connect a controller to show debug information.";
                    }
                    else if (Controller.NumberId == 2)
                    {
                        image_Controller2.Source = vImagePreloadIconControllerDark;
                        textblock_Controller2.Text = "No controller connected";
                        textblock_Controller2CodeName.Text = string.Empty;
                        textblock_LiveDebugInformation.Text = "Connect a controller to show debug information.";
                    }
                    else if (Controller.NumberId == 3)
                    {
                        image_Controller3.Source = vImagePreloadIconControllerDark;
                        textblock_Controller3.Text = "No controller connected";
                        textblock_Controller3CodeName.Text = string.Empty;
                        textblock_LiveDebugInformation.Text = "Connect a controller to show debug information.";
                    }
                });

                //Disconnect virtual controller
                if (vVirtualBusDevice != null)
                {
                    //Prepare empty xinput data
                    PrepareXInputDataEmpty(Controller);

                    //Send empty input to the virtual bus
                    vVirtualBusDevice.VirtualInput(ref Controller);

                    //Close the controller virtual events
                    SetAndCloseEvent(Controller.InputVirtualOverlapped.EventHandle);
                    SetAndCloseEvent(Controller.OutputVirtualOverlapped.EventHandle);

                    //Stop the controller loop tasks
                    await TaskStopLoop(Controller.InputControllerTask);
                    await TaskStopLoop(Controller.OutputControllerTask);
                    await TaskStopLoop(Controller.OutputVirtualTask);
                    await TaskStopLoop(Controller.OutputGyroTask);

                    //Disconnect the virtual controller
                    vVirtualBusDevice.VirtualUnplug(Controller.NumberId);
                    await Task.Delay(500);
                }

                //Stop and Close the Win Usb Device
                if (Controller.WinUsbDevice != null)
                {
                    try
                    {
                        //Dispose and stop connection with the controller
                        Controller.WinUsbDevice.CloseDevice();
                    }
                    catch { }
                }

                //Close and dispose the Hid Usb Device
                if (Controller.HidDevice != null)
                {
                    //Disconnect controller from bluetooth
                    try
                    {
                        if (Controller.Details.Wireless)
                        {
                            Controller.HidDevice.BluetoothDisconnect();
                        }
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
                }

                //Reset the controller status
                Controller.ResetControllerStatus();

                //Check if any controller is connected
                if (!vControllerAnyConnected() && Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "KeyboardCloseNoController")))
                {
                    Debug.WriteLine("No controller connected closing open popups.");
                    await HideOpenPopups();
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
                await StopControllerAsync(vController0, "all");
                await StopControllerAsync(vController1, "all");
                await StopControllerAsync(vController2, "all");
                await StopControllerAsync(vController3, "all");

                if (disconnectVirtualBus)
                {
                    vVirtualBusDevice.CloseDevice();
                    vVirtualBusDevice = null;
                }

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
                    Controller.WinUsbDevice = new WinUsbDevice(Guid.Empty, Controller.Details.Path, true, false);
                    if (!Controller.WinUsbDevice.Connected)
                    {
                        Debug.WriteLine("Invalid winusb open device, blocking: " + Controller.Details.DisplayName);
                        vControllerTempBlockPaths.Add(Controller.Details.Path);
                        return false;
                    }
                    else
                    {
                        //Set default controller variables
                        Controller.InputHeaderOffsetByte = 0;
                        Controller.InputButtonOffsetByte = 0;
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
                    Controller.HidDevice = new HidDevice(Controller.Details.Path, Controller.Details.ModelId, true, false);
                    if (!Controller.HidDevice.Connected)
                    {
                        Debug.WriteLine("Invalid hid open device, blocking: " + Controller.Details.DisplayName);
                        vControllerTempBlockPaths.Add(Controller.Details.Path);
                        return false;
                    }
                    else
                    {
                        //Get feature to make sure correct data is read
                        Controller.HidDevice.GetFeature(0x05);

                        //Set default controller variables
                        Controller.InputHeaderOffsetByte = 0;
                        Controller.InputButtonOffsetByte = 0;
                        Controller.InputReport = new byte[Controller.HidDevice.Capabilities.InputReportByteLength];
                        Controller.OutputReport = new byte[Controller.HidDevice.Capabilities.OutputReportByteLength];

                        //Read data from the controller
                        bool Readed = await Controller.HidDevice.ReadBytesFileTimeOut(Controller.InputReport, Controller.MilliSecondsAllowReadWrite);
                        if (!Readed)
                        {
                            Debug.WriteLine("Invalid hid read device, blocking: " + Controller.Details.DisplayName + " Len" + Controller.InputReport.Length + " Read" + Readed);
                            vControllerTempBlockPaths.Add(Controller.Details.Path);
                            return false;
                        }
                        else if (!Controller.InputReport.Take(5).Any(x => x != 0))
                        {
                            Debug.WriteLine("Invalid hid data: " + Controller.Details.DisplayName + " Len" + Controller.InputReport.Length + " Read" + Readed);
                            vControllerTempBlockPaths.Add(Controller.Details.Path);
                            return false;
                        }
                        else
                        {
                            Debug.WriteLine("Opened the hid controller: " + Controller.Details.DisplayName + ", exclusive: " + Controller.HidDevice.Exclusive);
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
    }
}