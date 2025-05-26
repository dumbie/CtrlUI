using ArnoldVinkCode;
using LibraryUsb;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
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
                string controllerNumberDisplay = Controller.NumberDisplay().ToString();

                //Open the selected controller
                if (!OpenController(Controller))
                {
                    Debug.WriteLine("Failed to initialize DirectInput for: " + Controller.Details.DisplayName);
                    await StopController(Controller, "failed", "Controller " + controllerNumberDisplay + " is no longer connected or failed.");
                    return false;
                }

                //Unplug and plugin virtual device
                bool virtualUnplug = await vVirtualBusDevice.VirtualUnplug(Controller.NumberVirtual());
                bool virtualPlugin = await vVirtualBusDevice.VirtualPlugin(Controller.NumberVirtual());
                Debug.WriteLine("Virtual device plugin result: " + virtualUnplug + " / " + virtualPlugin);

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
                async Task TaskActionOutputController()
                {
                    try
                    {
                        await LoopOutputController(Controller);
                    }
                    catch { }
                }
                AVActions.TaskStartLoop(TaskActionOutputController, Controller.OutputControllerTask);

                //Start output virtual task loop
                async Task TaskActionOutputVirtual()
                {
                    try
                    {
                        await LoopOutputVirtual(Controller);
                    }
                    catch { }
                }
                AVActions.TaskStartLoop(TaskActionOutputVirtual, Controller.OutputVirtualTask);

                //Start output gyroscope task loop
                async Task TaskActionOutputGyro()
                {
                    try
                    {
                        await LoopOutputGyro(Controller);
                    }
                    catch { }
                }
                AVActions.TaskStartLoop(TaskActionOutputGyro, Controller.OutputGyroscopeTask);

                return true;
            }
            catch
            {
                Debug.WriteLine("Failed initializing DirectInput for: " + Controller.Details.DisplayName);
                return false;
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