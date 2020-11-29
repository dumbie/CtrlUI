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
using static LibraryUsb.NativeMethods_Guid;

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
                    Controller.LastReadTicks = Stopwatch.GetTimestamp();

                    //Update the controller last active time
                    Controller.LastActiveTicks = Environment.TickCount;

                    //Set the controller supported profile
                    Controller.SupportedCurrent = vDirectControllersSupported.Where(x => x.ProductIDs.Any(z => z.ToLower() == Controller.Details.Profile.ProductID.ToLower() && x.VendorID.ToLower() == Controller.Details.Profile.VendorID.ToLower())).FirstOrDefault();
                    if (Controller.SupportedCurrent == null)
                    {
                        Debug.WriteLine("Unsupported controller detected, using default profile.");
                        Controller.SupportedCurrent = new ControllerSupported();
                    }

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
                        slider_TriggerRumbleStrength.IsEnabled = true;
                    }
                    else
                    {
                        slider_TriggerRumbleStrength.IsEnabled = false;
                    }
                    textblock_TriggerRumbleStrength.Text = textblock_TriggerRumbleStrength.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.TriggerRumbleStrength) + "%";
                    slider_TriggerRumbleStrength.Value = Controller.Details.Profile.TriggerRumbleStrength;

                    textblock_ControllerLedBrightness.Text = textblock_ControllerLedBrightness.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.LedBrightness) + "%";
                    slider_ControllerLedBrightness.Value = Controller.Details.Profile.LedBrightness;
                });
            }
            catch { }
        }

        //Stop the desired controller in task
        void StopControllerTask(ControllerStatus Controller, bool stopAll, string disconnectTag)
        {
            try
            {
                //Start the controller stop task
                async void TaskAction()
                {
                    try
                    {
                        await StopControllerAsync(Controller, stopAll, disconnectTag);
                    }
                    catch { }
                }
                AVActions.TaskStart(TaskAction);
            }
            catch { }
        }

        //Stop the desired controller as async
        async Task<bool> StopControllerAsync(ControllerStatus Controller, bool stopAll, string disconnectTag)
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
                        textblock_Controller0CodeName.Text = string.Empty;
                        textblock_LiveDebugInformation.Text = "No controller connected to debug";
                    }
                    else if (Controller.NumberId == 1)
                    {
                        image_Controller1.Source = vImagePreloadIconControllerDark;
                        textblock_Controller1.Text = "No controller connected";
                        textblock_Controller1CodeName.Text = string.Empty;
                        textblock_LiveDebugInformation.Text = "No controller connected to debug";
                    }
                    else if (Controller.NumberId == 2)
                    {
                        image_Controller2.Source = vImagePreloadIconControllerDark;
                        textblock_Controller2.Text = "No controller connected";
                        textblock_Controller2CodeName.Text = string.Empty;
                        textblock_LiveDebugInformation.Text = "No controller connected to debug";
                    }
                    else if (Controller.NumberId == 3)
                    {
                        image_Controller3.Source = vImagePreloadIconControllerDark;
                        textblock_Controller3.Text = "No controller connected";
                        textblock_Controller3CodeName.Text = string.Empty;
                        textblock_LiveDebugInformation.Text = "No controller connected to debug";
                    }
                });

                //Stop the controller loop task
                await TaskStopLoop(Controller.InputTask);

                //Reset the controller status
                Controller.ResetControllerStatus();

                //Disconnect virtual controller
                if (vVirtualBusDevice != null)
                {
                    //Prepare empty device data
                    PrepareXInputData(Controller, true);

                    //Send empty device data
                    vVirtualBusDevice.VirtualReadWrite(Controller.XInputData, Controller.XOutputData);
                    await Task.Delay(500);

                    //Disconnect the virtual controller
                    if (stopAll)
                    {
                        vVirtualBusDevice.VirtualUnplugAll();
                    }
                    else
                    {
                        vVirtualBusDevice.VirtualUnplug(Controller.NumberId);
                    }
                    await Task.Delay(500);

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
                        Controller.WinUsbDevice.CloseDevice();
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

                    Controller.HidDevice = null;
                }

                //Dispose the connected controller
                Controller.Details = null;

                //Check if any controller is connected
                if (!vControllerAnyConnected() && Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "KeyboardCloseNoController")))
                {
                    Debug.WriteLine("No controller connected closing open popups.");
                    await HideOpenPopups();
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
        async Task StopAllControllers(bool disconnectVirtualBus)
        {
            try
            {
                await StopControllerAsync(vController0, true, "all");
                await StopControllerAsync(vController1, true, "all");
                await StopControllerAsync(vController2, true, "all");
                await StopControllerAsync(vController3, true, "all");

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
                    Controller.HidDevice = new HidDevice(Controller.Details.Path, Controller.Details.HardwareId, true, false);
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
                            Debug.WriteLine("Opened the hid controller: " + Controller.Details.DisplayName + ", exclusive: " + Controller.HidDevice.IsExclusive);
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

        //Open virtual bus driver
        async Task<bool> OpenVirtualBusDriver()
        {
            try
            {
                vVirtualBusDevice = new WinUsbDevice(GuidClassScpVirtualBus, string.Empty, false, false);
                if (vVirtualBusDevice.Connected)
                {
                    Debug.WriteLine("Xbox drivers are installed.");
                    vVirtualBusDevice.VirtualUnplugAll();
                    await Task.Delay(500);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Xbox drivers not installed.");
                    return false;
                }
            }
            catch { }
            Debug.WriteLine("Xbox drivers not installed.");
            return false;
        }
    }
}