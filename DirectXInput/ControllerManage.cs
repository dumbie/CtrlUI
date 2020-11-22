using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.AVAudioDevice;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Monitor connected controllers
        async Task MonitorController()
        {
            try
            {
                //Check last controller disconnect time
                double lastDisconnectSeconds = (DateTime.Now - vControllerLastDisconnect).TotalMilliseconds;
                if (lastDisconnectSeconds <= 2500)
                {
                    Debug.WriteLine("A controller disconnected recently, delaying monitor.");
                    return;
                }

                //Load all the connected controllers
                await ControllerReceiveAllConnected();

                //Check if there is an active controller
                ControllerCheckActivated();
            }
            catch { }
        }

        //Monitor volume mute status
        void MonitorVolumeMute()
        {
            try
            {
                bool systemMuted = AudioMuteGetStatus();
                if (systemMuted != vControllerMuteLed)
                {
                    //Update the controller led
                    vControllerMuteLed = systemMuted;
                    SendXRumbleData(vController0, true, false, false);
                    SendXRumbleData(vController1, true, false, false);
                    SendXRumbleData(vController2, true, false, false);
                    SendXRumbleData(vController3, true, false, false);
                }
                else
                {
                    vControllerMuteLed = systemMuted;
                }
            }
            catch { }
        }

        //Validate the controller
        bool ControllerValidate(string vendorHexId, string productHexId, string controllerPath)
        {
            try
            {
                //Check for invalid or unknown controllers by id
                if (productHexId == "0x0000" && vendorHexId == "0x0000") { return false; } //Unknown
                if (productHexId == "0x028e" && vendorHexId == "0x045e") { return false; } //Xbox 360

                //Check if the controller is in temp block list
                if (vControllerTempBlockPaths.Contains(controllerPath))
                {
                    Debug.WriteLine("Controller is on temp block list: " + controllerPath);
                    return false;
                }

                //Check if the controller is in ignore list
                foreach (ControllerSupported ignoreCheck in vDirectControllersIgnored)
                {
                    string filterVendor = ignoreCheck.VendorID.ToLower();
                    string[] filterProducts = ignoreCheck.ProductIDs.Select(x => x.ToLower()).ToArray();
                    if (filterVendor == vendorHexId.ToLower() && filterProducts.Any(productHexId.ToLower().Contains))
                    {
                        Debug.WriteLine("Controller is on ignore list: " + controllerPath);
                        return false;
                    }
                }
            }
            catch { }
            return true;
        }

        //Connect with the controller
        async Task ControllerConnect(ControllerDetails ConnectedController)
        {
            try
            {
                //Check if the controller is already in use
                bool ControllerInuse = false;
                if (vController0.Connected && vController0.Details.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (vController1.Connected && vController1.Details.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (vController2.Connected && vController2.Details.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (vController3.Connected && vController3.Details.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (ControllerInuse) { return; }

                Debug.WriteLine("Found a connected " + ConnectedController.Type + " controller to use: " + ConnectedController.DisplayName);

                //Connect the controller to available slot
                if (!vController0.Connected)
                {
                    vController0.Details = ConnectedController;
                    bool controllerStarted = await StartControllerDirectInput(vController0);
                    if (controllerStarted)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            image_Controller0.Source = vImagePreloadIconControllerAccent;
                            textblock_Controller0.Text = vController0.Details.DisplayName;
                            textblock_Controller0CodeName.Text = vController0.SupportedCurrent.CodeName;
                        });
                    }
                }
                else if (!vController1.Connected)
                {
                    vController1.Details = ConnectedController;
                    bool controllerStarted = await StartControllerDirectInput(vController1);
                    if (controllerStarted)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            image_Controller1.Source = vImagePreloadIconControllerAccent;
                            textblock_Controller1.Text = vController1.Details.DisplayName;
                            textblock_Controller1CodeName.Text = vController1.SupportedCurrent.CodeName;
                        });
                    }
                }
                else if (!vController2.Connected)
                {
                    vController2.Details = ConnectedController;
                    bool controllerStarted = await StartControllerDirectInput(vController2);
                    if (controllerStarted)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            image_Controller2.Source = vImagePreloadIconControllerAccent;
                            textblock_Controller2.Text = vController2.Details.DisplayName;
                            textblock_Controller2CodeName.Text = vController2.SupportedCurrent.CodeName;
                        });
                    }
                }
                else if (!vController3.Connected)
                {
                    vController3.Details = ConnectedController;
                    bool controllerStarted = await StartControllerDirectInput(vController3);
                    if (controllerStarted)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            image_Controller3.Source = vImagePreloadIconControllerAccent;
                            textblock_Controller3.Text = vController3.Details.DisplayName;
                            textblock_Controller3CodeName.Text = vController3.SupportedCurrent.CodeName;
                        });
                    }
                }
            }
            catch { }
        }

        //Check if there is an actived controller
        void ControllerCheckActivated()
        {
            try
            {
                //Debug.WriteLine("There is currently no actived controller.");
                if (vController0.Connected && GetActiveController() == null) { ControllerActivate(vController0); }
                else if (vController1.Connected && GetActiveController() == null) { ControllerActivate(vController1); }
                else if (vController2.Connected && GetActiveController() == null) { ControllerActivate(vController2); }
                else if (vController3.Connected && GetActiveController() == null) { ControllerActivate(vController3); }
                else if (GetActiveController() == null)
                {
                    //Clear the current controller information
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_ActiveControllerType.Text = "Type";
                        txt_ActiveControllerLatency.Text = "Latency";
                        txt_ActiveControllerBattery.Text = "Battery";
                        txt_ActiveControllerName.Text = "No controller";
                    });
                }
            }
            catch { }
        }

        //Activate controller
        bool ControllerActivate(ControllerStatus Controller)
        {
            try
            {
                if (Controller.Connected && !Controller.Activated)
                {
                    Debug.WriteLine("Activating controller: " + Controller.NumberId);

                    if (GetActiveController() != null)
                    {
                        //Deactivate previous controller
                        GetActiveController().Activated = false;

                        //Show controller activated notification
                        string controllerNumberDisplay = (Controller.NumberId + 1).ToString();
                        NotificationDetails notificationDetails = new NotificationDetails();
                        notificationDetails.Icon = "Controller";
                        notificationDetails.Text = "Activated (" + controllerNumberDisplay + ")";
                        App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                    }

                    Controller.Activated = true;
                    ControllerUpdateSettingsInterface(Controller);
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Change the active controller to 0
        void Button_Controller0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControllerActivate(vController0);
            }
            catch { }
        }

        //Change the active controller to 1
        void Button_Controller1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControllerActivate(vController1);
            }
            catch { }
        }

        //Change the active controller to 2
        void Button_Controller2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControllerActivate(vController2);
            }
            catch { }
        }

        //Change the active controller to 3
        void Button_Controller3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControllerActivate(vController3);
            }
            catch { }
        }

        //Save the previous combobox selected item
        void Cb_Controller_Mouse_Down(object sender, EventArgs args)
        {
            try
            {
                ComboBox SelectedComboBox = (ComboBox)sender;
                vPrevComboboxIndex = SelectedComboBox.SelectedIndex;
            }
            catch { }
        }

        //Returns the active controller status
        ControllerStatus GetActiveController()
        {
            try
            {
                if (vController0.Activated) { return vController0; }
                else if (vController1.Activated) { return vController1; }
                else if (vController2.Activated) { return vController2; }
                else if (vController3.Activated) { return vController3; }
            }
            catch { }
            return null;
        }
    }
}